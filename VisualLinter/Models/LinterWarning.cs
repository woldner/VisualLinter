using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter.Models
{
    internal class LinterWarning
    {
        internal LinterMessage Message { get; }

        internal SnapshotSpan Span { get; }

        internal LinterWarning(LinterMessage message, SnapshotSpan span)
        {
            Message = message;
            Span = span;
        }

        internal LinterWarning CloneAndTranslateTo(ITextSnapshot newSnapshot)
        {
            var newSpan = Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            return newSpan.Length == Span.Length
                ? new LinterWarning(Message, newSpan)
                : null;
        }
    }
}