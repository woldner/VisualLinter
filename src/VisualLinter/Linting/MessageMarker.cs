using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter.Linting
{
    internal class MessageMarker
    {
        internal MessageMarker(EslintMessage message, SnapshotSpan span)
        {
            Message = message;
            Span = span;
        }

        internal EslintMessage Message { get; }
        internal SnapshotSpan Span { get; }

        internal MessageMarker Clone()
        {
            return new MessageMarker(Message, Span);
        }

        internal MessageMarker CloneAndTranslateTo(ITextSnapshot newSnapshot)
        {
            var newSpan = Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            return Span.Length == newSpan.Length
                ? new MessageMarker(Message, newSpan)
                : null;
        }
    }
}
