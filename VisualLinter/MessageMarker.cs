using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter
{
    internal class MessageMarker
    {
        public LinterMessage Message { get; }

        public SnapshotSpan Span { get; }

        internal MessageMarker(LinterMessage message, SnapshotSpan span)
        {
            Message = message;
            Span = span;
        }

        public MessageMarker CloneAndTranslateTo(ITextSnapshot newSnapshot)
        {
            var newSpan = Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            return newSpan.Length == Span.Length
                ? new MessageMarker(Message, newSpan)
                : null;
        }
    }
}