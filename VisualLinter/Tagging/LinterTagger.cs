using jwldnr.VisualLinter.Helpers;
using jwldnr.VisualLinter.Linting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jwldnr.VisualLinter.Tagging
{
    internal class LinterTagger : ITagger<IErrorTag>, IDisposable
    {
        internal SnapshotFactory Factory { get; }
        internal string FilePath { get; private set; }
        internal LinterSnapshot Snapshot { get; set; }

        private readonly ITextBuffer _buffer;
        private readonly ITextDocument _document;
        private readonly Linter _linter;
        private readonly TaggerProvider _provider;

        private ITextSnapshot _currentSnapshot;
        private NormalizedSnapshotSpanCollection _dirtySpans;

        internal LinterTagger(
            TaggerProvider provider,
            Linter linter,
            ITextBuffer buffer,
            ITextDocument document)
        {
            _provider = provider;
            _linter = linter;
            _buffer = buffer;
            _document = document;

            _currentSnapshot = buffer.CurrentSnapshot;
            _dirtySpans = new NormalizedSnapshotSpanCollection();

            FilePath = document.FilePath;
            Factory = new SnapshotFactory(new LinterSnapshot(FilePath, 0, new List<LinterMarker>()));

            Initialize();
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public void Dispose()
        {
            _document.FileActionOccurred -= OnFileActionOccurred;
            _buffer.ChangedLowPriority -= OnBufferChange;

            _provider.RemoveTagger(this);
        }

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (0 == spans.Count || null == Snapshot || false == Snapshot.Markers.Any())
                return Enumerable.Empty<ITagSpan<IErrorTag>>();

            try
            {
                return Snapshot.Markers
                    .Where(marker => spans.IntersectsWith(new NormalizedSnapshotSpanCollection(marker.Span)))
                    .Select(marker => new TagSpan<IErrorTag>(marker.Span, new LinterTag(marker.Message)));
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return Enumerable.Empty<ITagSpan<IErrorTag>>();
        }

        internal void UpdateMessages(IEnumerable<LinterMessage> messages)
        {
            var oldSnapshot = Factory.CurrentSnapshot;

            var markers = ProcessMessages(messages).Select(CreateMarker);
            var newSnapshot = new LinterSnapshot(FilePath, oldSnapshot.VersionNumber + 1, markers);

            SnapToNewSnapshot(newSnapshot);
        }

        private async Task AnalyzeAsync(string filePath)
        {
            if (null == VsixHelper.GetProjectItem(filePath))
                return;

            var source = _currentSnapshot.GetText();
            var messages = await LintAsync(filePath, source);

            UpdateMessages(messages);
        }

        private LinterMarker CreateMarker(LinterMessage message)
        {
            var line = message.Range.LineStart;

            var columnStart = message.Range.ColumnStart;
            var start = _currentSnapshot.GetPointInLine(line, columnStart);

            var columnEnd = message.Range.ColumnEnd;
            var end = _currentSnapshot.GetPointInLine(line, columnEnd);

            return new LinterMarker(new SnapshotSpan(start, end), message);
        }

        private MessageRange GetMessageRange(LinterMessage message)
        {
            try
            {
                var lineNumber = message.Line;

                if (lineNumber < 0)
                    lineNumber = 0;

                var lineCount = _currentSnapshot.LineCount;

                if (lineNumber > lineCount)
                    throw new ArgumentOutOfRangeException($"line number ({lineNumber}) greater than line count ({lineCount})");

                var line = _currentSnapshot.GetLineFromLineNumber(lineNumber);
                var lineText = line.GetText();
                var columnEnd = lineText.Length;

                var columnGiven = message.Column > -1;
                var columnStart = columnGiven ? message.Column : 0;

                if (columnGiven)
                {
                    var match = RegexHelper.GetWord(lineText.Substring(columnStart));
                    if (match.Success)
                        columnEnd = columnStart + match.Index + match.Length;
                }
                else
                {
                    var indentation = RegexHelper.GetIndentation(lineText);
                    if (indentation.Success)
                        columnStart = indentation.Length;
                }

                if (columnStart > lineText.Length)
                    throw new ArgumentOutOfRangeException($"column start ({columnStart}) greater than line length ({lineText.Length}) for line {lineNumber}");

                return new MessageRange
                {
                    ColumnEnd = columnEnd,
                    ColumnStart = columnStart,
                    LineEnd = lineNumber,
                    LineStart = lineNumber
                };
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return null;
        }

        private Task Initialize()
        {
            _document.FileActionOccurred += OnFileActionOccurred;
            _buffer.ChangedLowPriority += OnBufferChange;

            _provider.AddTagger(this);

            return AnalyzeAsync(FilePath);
        }

        private Task<IEnumerable<LinterMessage>> LintAsync(string filePath, string source)
        {
            return _linter.LintAsync(filePath, source);
        }

        private void OnBufferChange(object sender, TextContentChangedEventArgs e)
        {
            UpdateDirtySpans(e);

            var newSnapshot = TranslateWarningSpans();

            SnapToNewSnapshot(newSnapshot);
        }

        private async void OnFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
        {
            if (0 != (e.FileActionType & FileActionTypes.DocumentRenamed))
            {
                _provider.Rename(FilePath, e.FilePath);
                FilePath = e.FilePath;
            }
            else if (0 != (e.FileActionType & FileActionTypes.ContentSavedToDisk))
            {
                await AnalyzeAsync(FilePath);
            }
        }

        private IEnumerable<LinterMessage> ProcessMessages(IEnumerable<LinterMessage> messages)
        {
            foreach (var message in messages)
            {
                var messageColumn = message.Column;
                var messageLine = message.Line;

                var messageEndLine = message.EndLine;
                var messageEndColumn = message.EndColumn;

                if (messageEndColumn.HasValue && messageEndLine.HasValue)
                {
                    _currentSnapshot.ValidatePoint(messageLine, messageColumn);
                    _currentSnapshot.ValidatePoint(messageEndLine.Value, messageEndColumn.Value);

                    message.Range = new MessageRange
                    {
                        ColumnEnd = messageEndColumn.Value,
                        ColumnStart = messageColumn,
                        LineEnd = messageEndLine.Value,
                        LineStart = messageLine
                    };
                }
                else
                {
                    message.Range = GetMessageRange(message);
                }

                yield return message;
            }
        }

        private void SnapToNewSnapshot(LinterSnapshot snapshot)
        {
            var factory = Factory;

            factory.UpdateResults(snapshot);

            _provider.UpdateAllSinks(factory);

            UpdateWarnings(_currentSnapshot, snapshot);

            Snapshot = snapshot;
        }

        private LinterSnapshot TranslateWarningSpans()
        {
            var oldSnapshot = Factory.CurrentSnapshot;

            var newWarnings = oldSnapshot.Markers
                .Select(marker => marker.CloneAndTranslateTo(marker, _currentSnapshot))
                .Where(clone => null != clone);

            return new LinterSnapshot(FilePath, oldSnapshot.VersionNumber + 1, newWarnings);
        }

        private void UpdateDirtySpans(TextContentChangedEventArgs e)
        {
            _currentSnapshot = e.After;

            var newDirtySpans = _dirtySpans.CloneAndTrackTo(e.After, SpanTrackingMode.EdgeInclusive);

            newDirtySpans = e.Changes
                .Aggregate(newDirtySpans, (current, change) => NormalizedSnapshotSpanCollection.Union(
                    current, new NormalizedSnapshotSpanCollection(e.After, change.NewSpan)));

            _dirtySpans = newDirtySpans;
        }

        private void UpdateWarnings(ITextSnapshot currentSnapshot, LinterSnapshot snapshot)
        {
            var oldSnapshot = Snapshot;

            var handler = TagsChanged;
            if (null == handler)
                return;

            var start = int.MaxValue;
            var end = int.MinValue;

            if (null != oldSnapshot && 0 < oldSnapshot.Count)
            {
                start = oldSnapshot.Markers.First().Span.Start
                    .TranslateTo(currentSnapshot, PointTrackingMode.Negative);
                end = oldSnapshot.Markers.Last().Span.End
                    .TranslateTo(currentSnapshot, PointTrackingMode.Positive);
            }

            if (0 < snapshot.Count)
            {
                start = Math.Min(start, snapshot.Markers.First().Span.Start.Position);
                end = Math.Max(end, snapshot.Markers.Last().Span.End.Position);
            }

            if (start >= end)
                return;

            handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(
                currentSnapshot, Span.FromBounds(start, end))));
        }
    }
}