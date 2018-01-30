using jwldnr.VisualLinter.Helpers;
using jwldnr.VisualLinter.Linting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace jwldnr.VisualLinter.Tagging
{
    internal class LinterTagger : ITagger<IErrorTag>, IDisposable
    {
        private readonly ITextBuffer _buffer;
        private readonly ITextDocument _document;
        private readonly TaggerProvider _provider;

        private ITextSnapshot _currentSnapshot;
        private NormalizedSnapshotSpanCollection _dirtySpans;
        private CancellationTokenSource _source;
        internal SnapshotFactory Factory { get; }
        internal string FilePath { get; private set; }
        internal LinterSnapshot Snapshot { get; set; }

        internal LinterTagger(
            TaggerProvider provider,
            ITextBuffer buffer,
            ITextDocument document)
        {
            _provider = provider;
            _buffer = buffer;
            _document = document;

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

        internal void UpdateMessages(IEnumerable<EslintMessage> messages)
        {
            var oldSnapshot = Factory.CurrentSnapshot;

            var markers = ProcessMessages(messages).Select(CreateMarker);
            var newSnapshot = new LinterSnapshot(FilePath, oldSnapshot.VersionNumber + 1, markers);

            SnapToNewSnapshot(newSnapshot);
        }

        private static MessageRange GetRange(SnapshotPoint start, SnapshotPoint end, int line)
        {
            if (start > end)
                throw new ArgumentOutOfRangeException($"start ({start.Position}) greater than end ({end.Position}) for line {line}");

            return new MessageRange
            {
                Start = start,
                End = end
            };
        }

        private async Task Analyze(string filePath)
        {
            if (null == VsixHelper.GetProjectItem(filePath))
                return;

            Cancel();

            _source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await _provider.Analyze(filePath, _source.Token)
                .ConfigureAwait(false);
        }

        private void Cancel()
        {
            try
            {
                _source?.Cancel();
            }
            catch (Exception e)
            {
                OutputWindowHelper.WriteLine(e.Message);
            }
            finally
            {
                _source?.Dispose();
                _source = null;
            }
        }

        private MessageMarker CreateMarker(EslintMessage message)
        {
            var start = new SnapshotPoint(_currentSnapshot, message.Range.Start);
            var end = new SnapshotPoint(_currentSnapshot, message.Range.End);

            return new MessageMarker(message, new SnapshotSpan(start, end));
        }

        private void Initialize()
        {
            _document.FileActionOccurred += OnFileActionOccurred;
            _buffer.ChangedLowPriority += OnBufferChange;

            _provider.AddTagger(this, () => Analyze(FilePath));
        }

        private void OnBufferChange(object sender, TextContentChangedEventArgs e)
        {
            Cancel();

            UpdateDirtySpans(e);

            var newSnapshot = TranslateWarningSpans();

            SnapToNewSnapshot(newSnapshot);
        }

        private async void OnFileActionOccurred(object sender, TextDocumentFileActionEventArgs e)
        {
            switch (e.FileActionType)
            {
                case FileActionTypes.ContentSavedToDisk:
                case FileActionTypes.ContentLoadedFromDisk:
                    await Analyze(FilePath).ConfigureAwait(false);
                    break;

                case FileActionTypes.DocumentRenamed:
                    Rename(FilePath, e.FilePath);
                    break;

                default:
                    OutputWindowHelper.WriteLine("info: unrecognized file action type");
                    break;
            }
        }

        private IEnumerable<EslintMessage> ProcessMessages(IEnumerable<EslintMessage> messages)
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

        private void Rename(string oldPath, string newPath)
        {
            _provider.Rename(oldPath, newPath);

            FilePath = newPath;
        }

        private void SnapToNewSnapshot(LinterSnapshot snapshot)
        {
            var factory = Factory;

            factory.UpdateMarkers(snapshot);

            _provider.UpdateAllSinks(factory);

            UpdateMarkers(_currentSnapshot, snapshot);

            Snapshot = snapshot;
        }

        private LinterSnapshot TranslateWarningSpans()
        {
            var oldSnapshot = Factory.CurrentSnapshot;

            var newWarnings = oldSnapshot.Markers
                .Select(marker => marker.CloneAndTranslateTo(_currentSnapshot))
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

        private void UpdateMarkers(ITextSnapshot currentSnapshot, LinterSnapshot snapshot)
        {
            var oldSnapshot = Snapshot;

            var handler = TagsChanged;
            if (null == handler)
                return;

            var start = int.MaxValue;
            var end = int.MinValue;

            if (null != oldSnapshot && 0 < oldSnapshot.Count)
            {
                start = oldSnapshot.Markers
                    .Select(marker => marker.Span.Start.TranslateTo(currentSnapshot, PointTrackingMode.Negative))
                    .Min();
                end = oldSnapshot.Markers
                    .Select(marker => marker.Span.End.TranslateTo(currentSnapshot, PointTrackingMode.Positive))
                    .Max();
            }

            if (0 < snapshot.Count)
            {
                start = Math.Min(start, snapshot.Markers.Select(marker => marker.Span.Start.Position)
                    .Min());
                end = Math.Max(end, snapshot.Markers.Select(marker => marker.Span.End.Position)
                    .Max());
            }

            if (start > end)
                return;

            handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(
                currentSnapshot, Span.FromBounds(start, end))));
        }
    }
}
