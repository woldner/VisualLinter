using jwldnr.VisualLinter.Helpers;
using jwldnr.VisualLinter.Linting;
using jwldnr.VisualLinter.Models;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jwldnr.VisualLinter.Tagging
{
    internal class Tagger : ITagger<IErrorTag>, IDisposable
    {
        internal SnapshotFactory Factory { get; }
        internal string FilePath { get; private set; }
        internal LintSnapshot Snapshot { get; set; }

        private readonly ITextBuffer _buffer;
        private readonly ITextDocument _document;
        private readonly Linter _linter;
        private readonly TaggerProvider _provider;

        private ITextSnapshot _currentSnapshot;
        private NormalizedSnapshotSpanCollection _dirtySpans;

        internal Tagger(
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
            Factory = new SnapshotFactory(new LintSnapshot(FilePath, 0, new List<LintWarning>()));

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
            if (0 == spans.Count || null == Snapshot || !Snapshot.Warnings.Any())
                return Enumerable.Empty<ITagSpan<IErrorTag>>();

            try
            {
                return Snapshot.Warnings
                    .Where(warning => spans.IntersectsWith(new NormalizedSnapshotSpanCollection(warning.Span)))
                    .Select(warning => new TagSpan<IErrorTag>(warning.Span, new LintTag(warning.Message)));
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return Enumerable.Empty<ITagSpan<IErrorTag>>();
        }

        internal void UpdateMessages(IEnumerable<LintMessage> messages)
        {
            var oldSnapshot = Factory.CurrentSnapshot;

            var warnings = GetMessageRanges(messages).Where(IsValidRange).Select(CreateWarning);
            var newSnapshot = new LintSnapshot(FilePath, oldSnapshot.VersionNumber + 1, warnings);

            SnapToNewSnapshot(newSnapshot);
        }

        private async Task AnalyzeAsync(string filePath)
        {
            var messages = await LintAsync(filePath);
            UpdateMessages(messages);
        }

        private LintWarning CreateWarning(LintMessage message)
        {
            var start = new SnapshotPoint(_currentSnapshot, message.Range.StartColumn);
            var end = new SnapshotPoint(_currentSnapshot, message.Range.EndColumn);

            return new LintWarning(new SnapshotSpan(start, end), message);
        }

        private MessageRange GetMessageRange(LintMessage message)
        {
            try
            {
                var lineNumber = message.Line - 1;
                var column = message.Column - 1;

                if (lineNumber < 0)
                    lineNumber = 0;

                var lineCount = _currentSnapshot.LineCount;
                if (lineNumber > lineCount)
                    throw new ArgumentOutOfRangeException(
                        $"Line number ({lineNumber}) greater than line count ({lineCount})");

                var line = _currentSnapshot.GetLineFromLineNumber(lineNumber);
                var lineText = line.GetText();

                int endColumn = line.End;
                var startColumn = line.Start.Add(column);

                if (message.EndColumn.HasValue)
                {
                    var value = message.EndColumn.Value - 1;
                    var length = value - column;
                    endColumn = startColumn.Add(length);
                }
                else
                {
                    var match = RegexHelper.GetWord(lineText.Substring(column));
                    if (match.Success)
                        endColumn = startColumn.Add(match.Index).Add(match.Length);
                }

                if (startColumn > endColumn)
                    throw new ArgumentOutOfRangeException(
                        $"Start column ({startColumn}) greater than end column ({endColumn}) for line {lineNumber}");

                return new MessageRange
                {
                    LineNumber = lineNumber,
                    StartColumn = startColumn,
                    EndColumn = endColumn
                };
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }

            return null;
        }

        private IEnumerable<LintMessage> GetMessageRanges(IEnumerable<LintMessage> messages)
        {
            foreach (var message in messages)
            {
                message.Range = GetMessageRange(message);
                yield return message;
            }
        }

        private Task Initialize()
        {
            _document.FileActionOccurred += OnFileActionOccurred;
            _buffer.ChangedLowPriority += OnBufferChange;

            _provider.AddTagger(this);

            return AnalyzeAsync(FilePath);
        }

        private bool IsValidRange(LintMessage message)
        {
            var range = message.Range;

            if (null == range)
                return false;

            return range.LineNumber >= 0 && range.LineNumber <= _currentSnapshot.LineCount &&
                range.EndColumn <= _currentSnapshot.Length;
        }

        private Task<IEnumerable<LintMessage>> LintAsync(string filePath)
        {
            return _linter.LintAsync(filePath);
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

        private void SnapToNewSnapshot(LintSnapshot snapshot)
        {
            var factory = Factory;

            factory.UpdateResults(snapshot);

            _provider.UpdateAllSinks(factory);

            UpdateWarnings(_currentSnapshot, snapshot);

            Snapshot = snapshot;
        }

        private LintSnapshot TranslateWarningSpans()
        {
            var oldSnapshot = Factory.CurrentSnapshot;

            var newWarnings = oldSnapshot.Warnings
                .Select(warning => warning.CloneAndTranslateTo(warning, _currentSnapshot))
                .Where(clone => null != clone);

            return new LintSnapshot(FilePath, oldSnapshot.VersionNumber + 1, newWarnings);
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

        private void UpdateWarnings(ITextSnapshot currentSnapshot, LintSnapshot snapshot)
        {
            var oldSnapshot = Snapshot;

            var handler = TagsChanged;
            if (null == handler)
                return;

            var start = int.MaxValue;
            var end = int.MinValue;

            if (null != oldSnapshot && 0 < oldSnapshot.Count)
            {
                start = oldSnapshot.Warnings.First().Span.Start
                    .TranslateTo(currentSnapshot, PointTrackingMode.Negative);
                end = oldSnapshot.Warnings.Last().Span.End
                    .TranslateTo(currentSnapshot, PointTrackingMode.Positive);
            }

            if (0 < snapshot.Count)
            {
                start = Math.Min(start, snapshot.Warnings.First().Span.Start.Position);
                end = Math.Max(end, snapshot.Warnings.Last().Span.End.Position);
            }

            if (end > start)
                handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(
                    currentSnapshot, Span.FromBounds(start, end))));
        }
    }
}