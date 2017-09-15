using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter.Linting
{
    internal class MessageMarker
    {
        internal EslintMessage Message { get; }

        internal SnapshotSpan Span { get; }

        internal MessageMarker(SnapshotSpan span, EslintMessage message)
        {
            Message = message;
            Span = span;
        }

        internal static MessageMarker Clone(MessageMarker marker)
        {
            return new MessageMarker(marker.Span, marker.Message);
        }

        internal MessageMarker CloneAndTranslateTo(MessageMarker marker, ITextSnapshot newSnapshot)
        {
            var newSpan = marker.Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            return newSpan.Length == marker.Span.Length
                ? new MessageMarker(newSpan, marker.Message)
                : null;
        }
    }
}