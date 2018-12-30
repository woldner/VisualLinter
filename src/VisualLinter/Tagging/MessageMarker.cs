using jwldnr.VisualLinter.Linting;
using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter.Tagging
{
    internal class MessageMarker
    {
        internal MessageMarker(LinterMessage message, SnapshotSpan span)
        {
            Message = message;
            Span = span;
        }

        internal LinterMessage Message { get; }
        internal SnapshotSpan Span { get; }

        internal MessageMarker CloneAndTranslateTo(ITextSnapshot newSnapshot)
        {
            var newSpan = Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            return Span.Length == newSpan.Length
                ? new MessageMarker(Message, newSpan)
                : null;
        }
    }
}
