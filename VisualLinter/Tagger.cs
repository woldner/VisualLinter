using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jwldnr.VisualLinter
{
    internal class Tagger : ITagger<IErrorTag>, IDisposable
    {
        internal SnapshotFactory Factory { get; }
        internal string FilePath { get; private set; }
        internal LinterSnapshot Snapshot { get; set; }

        private readonly ITextBuffer _buffer;
        private readonly ITextDocument _document;
        private readonly TaggerProvider _provider;

        private ITextSnapshot _currentSnapshot;
        private NormalizedSnapshotSpanCollection _dirtySpans;

        internal Tagger(ITextBuffer buffer, ITextDocument document, TaggerProvider provider)
        {
            _provider = provider;
            _document = document;
            _buffer = buffer;

            _currentSnapshot = buffer.CurrentSnapshot;
            _dirtySpans = new NormalizedSnapshotSpanCollection();

            FilePath = document.FilePath;
            Factory = new SnapshotFactory(new LinterSnapshot(FilePath, 0, new List<MessageMarker>()));

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
            if (null == Snapshot)
                return Enumerable.Empty<ITagSpan<IErrorTag>>();

            return Snapshot.Markers
                .Select(marker => marker)
                .Where(marker => spans.IntersectsWith(marker.Span))
                .Select(marker => new TagSpan<IErrorTag>(marker.Span, GetErrorTag(marker.Message)));
        }

        internal void UpdateMessages(IEnumerable<LinterMessage> messages)
        {
            var oldSnapshot = Factory.CurrentSnapshot;

            var newMarkers = GetRanges(messages).Where(IsValidRange).Select(AddMarker);
            var newSnapshot = new LinterSnapshot(FilePath, oldSnapshot.VersionNumber + 1, newMarkers);

            SnapToNewSnapshot(newSnapshot);
        }

        private static IErrorTag GetErrorTag(LinterMessage message)
        {
            return new ErrorTag(
                GetErrorType(message.IsFatal),
                GetToolTipContent(message.Message, message.RuleId));
        }

        private static string GetErrorType(bool isFatal)
        {
            return isFatal
                ? PredefinedErrorTypeNames.SyntaxError
                : PredefinedErrorTypeNames.Warning;
        }

        private static object GetToolTipContent(string message, string ruleId)
        {
            return $"{message} ({ruleId})";
        }

        private MessageMarker AddMarker(LinterMessage message)
        {
            var start = new SnapshotPoint(_currentSnapshot, message.Range.StartColumn);
            var end = new SnapshotPoint(_currentSnapshot, message.Range.EndColumn);

            return new MessageMarker(message, new SnapshotSpan(start, end));
        }

        private MessageRange GetRange(LinterMessage message)
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

        private IEnumerable<LinterMessage> GetRanges(IEnumerable<LinterMessage> messages)
        {
            foreach (var message in messages)
            {
                message.Range = GetRange(message);
                yield return message;
            }
        }

        private Task Initialize()
        {
            _document.FileActionOccurred += OnFileActionOccurred;
            _buffer.ChangedLowPriority += OnBufferChange;

            _provider.AddTagger(this);

            return _provider.AnalyzeAsync(FilePath);
        }

        private bool IsValidRange(LinterMessage message)
        {
            var range = message.Range;

            if (null == range)
                return false;

            return range.LineNumber >= 0 && range.LineNumber <= _currentSnapshot.LineCount &&
                   range.EndColumn <= _currentSnapshot.Length;
        }

        private void OnBufferChange(object sender, TextContentChangedEventArgs e)
        {
            UpdateDirtySpans(e);

            var newMarkers = TranslateMarkerSpans();

            SnapToNewSnapshot(newMarkers);
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
                await _provider.AnalyzeAsync(FilePath);
            }
        }

        private void SnapToNewSnapshot(LinterSnapshot snapshot)
        {
            Factory.UpdateResults(snapshot);

            _provider.UpdateAllSinks();

            UpdateMarkers(_currentSnapshot, snapshot);

            Snapshot = snapshot;
        }

        private LinterSnapshot TranslateMarkerSpans()
        {
            var oldSnapshot = Factory.CurrentSnapshot;
            var newMarkers = oldSnapshot.Markers
                .Select(marker => marker.CloneAndTranslateTo(_currentSnapshot))
                .Where(clone => null != clone);

            return new LinterSnapshot(FilePath, oldSnapshot.VersionNumber + 1, newMarkers);
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

        private void UpdateMarkers(ITextSnapshot currentSnapshot, LinterSnapshot snapshot)
        {
            var oldSnapshot = Snapshot;

            var handler = TagsChanged;
            if (null == handler)
                return;

            var start = int.MaxValue;
            var end = int.MinValue;

            if (null != oldSnapshot && oldSnapshot.Count > 0)
            {
                start = oldSnapshot.Markers.First().Span.Start
                    .TranslateTo(currentSnapshot, PointTrackingMode.Negative);
                end = oldSnapshot.Markers.Last().Span.End
                    .TranslateTo(currentSnapshot, PointTrackingMode.Positive);
            }

            if (snapshot.Count > 0)
            {
                start = Math.Min(start, snapshot.Markers.First().Span.Start.Position);
                end = Math.Max(end, snapshot.Markers.Last().Span.End.Position);
            }

            if (start < end)
                handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(
                    currentSnapshot, Span.FromBounds(start, end))));
        }
    }
}