using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter
{
    internal class MessageMarker
    {
        internal LinterMessage Message { get; }
        internal SnapshotSpan Span { get; }

        internal MessageMarker(LinterMessage message, SnapshotSpan span)
        {
            Message = message;
            Span = span;
        }

        internal MessageMarker CloneAndTranslateTo(ITextSnapshot newSnapshot)
        {
            var newSpan = Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            return newSpan.Length == Span.Length
                ? new MessageMarker(Message, newSpan)
                : null;
        }
    }
}