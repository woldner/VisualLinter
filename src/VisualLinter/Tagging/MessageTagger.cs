using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace jwldnr.VisualLinter.Tagging
{
    internal class MessageTagger : ITagger<IErrorTag>, IDisposable
    {
        private readonly MessageTracker _tracker;

        private MessagesSnapshot _snapshot;

        internal MessageTagger(MessageTracker tracker)
        {
            _snapshot = tracker.LastSnapshot;
            _tracker = tracker;

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

            return _snapshot.Markers
                .Where(marker => spans.IntersectsWith(marker.Span))
                .Select(marker => new TagSpan<IErrorTag>(marker.Span, new MessageTag(marker.Message)));
        }

        public void UpdateMarkers(MessagesSnapshot snapshot, SnapshotSpan? span)
        {
            _snapshot = snapshot;

            var handler = TagsChanged;
            if (null != handler && span.HasValue)
                handler(this, new SnapshotSpanEventArgs(span.Value));
        }
    }
}
