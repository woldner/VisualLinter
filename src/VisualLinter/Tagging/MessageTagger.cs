using System;
using System.Collections.Generic;
using System.Linq;
using jwldnr.VisualLinter.Helpers;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace jwldnr.VisualLinter.Tagging
{
    internal class MessageTagger : ITagger<IErrorTag>, IDisposable
    {
        private MessagesSnapshot _snapshot;

        private readonly MessageTracker _tracker;
        private readonly ILogger _logger;

        internal MessageTagger(
            MessageTracker tracker,
            ILogger logger)
        {
            _snapshot = tracker.LastSnapshot;
            _tracker = tracker;
            _logger = logger;

            tracker.AddTagger(this);
        }

        public void Dispose()
        {
            _tracker.RemoveTagger(this);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (null == _snapshot)
                return Enumerable.Empty<ITagSpan<IErrorTag>>();

            try
            {
                return _snapshot.Markers
                    .Where(marker => spans.IntersectsWith(marker.Span))
                    .Select(marker => new TagSpan<IErrorTag>(marker.Span, new MessageTag(marker.Message)));
            }
            catch (Exception e)
            {
                _logger.WriteLine(e.Message);
            }

            return Enumerable.Empty<ITagSpan<IErrorTag>>();
        }

        public void UpdateMarkers(MessagesSnapshot snapshot, SnapshotSpan? span)
        {
            _snapshot = snapshot;

            var handler = TagsChanged;
            if (null != handler && span.HasValue)
                handler(this, new SnapshotSpanEventArgs(span.Value));
        }

        //private async Task Analyze(string filePath)
        //{
        //    if (null == VsixHelper.GetProjectItem(filePath))
        //        return;

        //    Cancel();

        //    _source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        //    await _provider.Analyze(filePath, _source.Token)
        //        .ConfigureAwait(false);
        //}

        //private void Cancel()
        //{
        //    try
        //    {
        //        _source?.Cancel();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.WriteLine(e.Message);
        //    }
        //    finally
        //    {
        //        _source?.Dispose();
        //        _source = null;
        //    }
        //}
        

        //private void Initialize()
        //{
        //    _document.FileActionOccurred += OnFileActionOccurred;
        //    _buffer.ChangedLowPriority += OnBufferChange;

        //    _provider.AddTagger(this, () => Analyze(FilePath));
        //}

        //private void OnBufferChange(object sender, TextContentChangedEventArgs e)
        //{
        //    Cancel();

        //    UpdateDirtySpans(e);

        //    var newSnapshot = TranslateWarningSpans();

        //    SnapToNewSnapshot(newSnapshot);
        //}
    }
}
