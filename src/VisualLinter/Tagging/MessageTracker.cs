using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using jwldnr.VisualLinter.Helpers;
using jwldnr.VisualLinter.Linting;
using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter.Tagging
{
    public interface IMessageTracker
    {
        void Accept(string filePath, IEnumerable<LinterMessage> messages);
    }

    internal sealed class MessageTracker : IMessageTracker
    {
        private readonly ITextDocument _document;
        private readonly ILogger _logger;
        private readonly TaggerProvider _provider;
        private readonly ISet<MessageTagger> _taggers = new HashSet<MessageTagger>();
        private readonly ITextBuffer _textBuffer;

        private ITextSnapshot _currentSnapshot;
        private NormalizedSnapshotSpanCollection _dirtySpans;
        private CancellationTokenSource _source;

        internal MessageTracker(
            ITextDocument document,
            ILogger logger,
            TaggerProvider provider)
        {
            _document = document;
            _logger = logger;
            _provider = provider;

            FilePath = document.FilePath;
            _textBuffer = document.TextBuffer;
            _currentSnapshot = document.TextBuffer.CurrentSnapshot;

            Factory = new SnapshotFactory(new MessagesSnapshot(FilePath, 0, new List<MessageMarker>()));

            document.FileActionOccurred += FileActionOccurred;
        }

        internal string FilePath { get; private set; }

        internal SnapshotFactory Factory { get; }
        internal MessagesSnapshot LastSnapshot { get; private set; }

        public void Accept(string filePath, IEnumerable<LinterMessage> messages)
        {
            if (filePath != FilePath)
                return;

            var markers = ProcessMessages(messages).Select(CreateMarker);
            UpdateMessages(markers);
        }

        private MessageMarker CreateMarker(LinterMessage message)
        {
            var start = new SnapshotPoint(_currentSnapshot, message.Range.Start);
            var end = new SnapshotPoint(_currentSnapshot, message.Range.End);

            return new MessageMarker(message, new SnapshotSpan(start, end));
        }

        private static MessageRange GetRange(SnapshotPoint start, SnapshotPoint end, int line)
        {
            if (start > end)
                throw new ArgumentOutOfRangeException(
                    $"start ({start.Position}) greater than end ({end.Position}) for line {line}");

            return new MessageRange
            {
                Start = start,
                End = end
            };
        }

        private IEnumerable<LinterMessage> ProcessMessages(IEnumerable<LinterMessage> messages)
        {
            foreach (var message in messages)
            {
                var lineNumber = message.Line;
                var lineCount = _currentSnapshot.LineCount;

                if (lineNumber > lineCount)
                    throw new ArgumentOutOfRangeException($"line ({lineNumber}) greater than line count ({lineCount})");

                var startLine = _currentSnapshot.GetLineFromLineNumber(lineNumber);
                var start = startLine.Start.Add(message.Column);

                if (message.EndLine.HasValue && message.EndColumn.HasValue)
                {
                    var endLine = _currentSnapshot.GetLineFromLineNumber(message.EndLine.Value);
                    var end = endLine.Start.Add(message.EndColumn.Value);

                    message.Range = GetRange(start, end, message.EndLine.Value);
                }
                else
                {
                    var lineText = startLine.GetText();
                    var end = startLine.End;

                    var match = RegexHelper.GetWord(lineText.Substring(message.Column));
                    if (match.Success)
                        end = start.Add(match.Index).Add(match.Length);

                    message.Range = GetRange(start, end, lineNumber);
                }

                yield return message;
            }
        }

        public void AddTagger(MessageTagger tagger)
        {
            _taggers.Add(tagger);

            if (_taggers.Count != 1)
                return;

            _textBuffer.ChangedLowPriority += OnBufferChange;

            _dirtySpans =
                new NormalizedSnapshotSpanCollection(new SnapshotSpan(_currentSnapshot, 0, _currentSnapshot.Length));

            _provider.AddTracker(this);

            Analyze(FilePath);
        }

        public void RemoveTagger(MessageTagger tagger)
        {
            _taggers.Remove(tagger);

            if (_taggers.Count != 0)
                return;

            _document.FileActionOccurred -= FileActionOccurred;

            _textBuffer.ChangedLowPriority -= OnBufferChange;
            _textBuffer.Properties.RemoveProperty(typeof(MessageTracker));

            _provider.RemoveTracker(this);
        }

        private void FileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
        {
            switch (e.FileActionType)
            {
                case FileActionTypes.DocumentRenamed:
                    FilePath = e.FilePath;
                    UpdateMessages(Factory.CurrentSnapshot.Markers ?? Enumerable.Empty<MessageMarker>());
                    break;

                case FileActionTypes.ContentLoadedFromDisk when _taggers.Count > 0:
                case FileActionTypes.ContentSavedToDisk when _taggers.Count > 0:
                    Analyze(FilePath);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Cancel()
        {
            try
            {
                _source?.Cancel();
            }
            catch (Exception e)
            {
                _logger.WriteLine(e.Message);
            }
            finally
            {
                _source?.Dispose();
                _source = null;
            }
        }

        private void Analyze(string filePath)
        {
            if (null == VsixHelper.GetProjectItem(filePath))
                return;

            Cancel();

            _source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var token = _source.Token;

            _provider.Analyze(this, filePath, token);
        }

        private void UpdateMessages(IEnumerable<MessageMarker> markers)
        {
            var oldSnapshot = Factory.CurrentSnapshot;
            var newSnapshot = new MessagesSnapshot(FilePath, oldSnapshot.VersionNumber + 1, markers);

            SnapToNewSnapshot(newSnapshot);
        }

        private void OnBufferChange(object sender, TextContentChangedEventArgs e)
        {
            Cancel();

            UpdateDirtySpans(e);

            var snapshot = TranslateMarkerSpans();

            SnapToNewSnapshot(snapshot);
        }

        private void UpdateDirtySpans(TextContentChangedEventArgs e)
        {
            _currentSnapshot = e.After;

            var newDirtySpans = _dirtySpans.CloneAndTrackTo(e.After, SpanTrackingMode.EdgeInclusive);

            newDirtySpans = e.Changes.Aggregate(newDirtySpans,
                (current, change) => NormalizedSnapshotSpanCollection.Union(current,
                    new NormalizedSnapshotSpanCollection(e.After, change.NewSpan)));

            _dirtySpans = newDirtySpans;
        }

        private MessagesSnapshot TranslateMarkerSpans()
        {
            var oldSnapshot = Factory.CurrentSnapshot;

            var markers = oldSnapshot.Markers
                .Select(marker => marker.CloneAndTranslateTo(_currentSnapshot))
                .Where(clone => clone != null);

            return new MessagesSnapshot(FilePath, oldSnapshot.VersionNumber + 1, markers);
        }

        private void SnapToNewSnapshot(MessagesSnapshot snapshot)
        {
            Factory.UpdateSnapshot(snapshot);

            _provider.UpdateAllSinks();

            var span = GetAffectedSpan(LastSnapshot, snapshot);
            foreach (var tagger in _taggers) tagger.UpdateMarkers(snapshot, span);

            LastSnapshot = snapshot;
        }

        private SnapshotSpan? GetAffectedSpan(MessagesSnapshot oldSnapshot, MessagesSnapshot newSnapshot)
        {
            var start = int.MaxValue;
            var end = int.MinValue;

            if (null != oldSnapshot && 0 < oldSnapshot.Count)
            {
                start = oldSnapshot.Markers.Select(marker =>
                    marker.Span.Start.TranslateTo(_currentSnapshot, PointTrackingMode.Negative)).Min();
                end = oldSnapshot.Markers.Select(marker =>
                    marker.Span.End.TranslateTo(_currentSnapshot, PointTrackingMode.Positive)).Max();
            }

            if (null != newSnapshot && 0 < newSnapshot.Count)
            {
                start = Math.Min(start, newSnapshot.Markers.Select(marker => marker.Span.Start.Position).Min());
                end = Math.Max(end, newSnapshot.Markers.Select(marker => marker.Span.End.Position).Max());
            }

            if (start > end)
                return null;

            return new SnapshotSpan(_currentSnapshot, Span.FromBounds(start, end));
        }
    }
}
