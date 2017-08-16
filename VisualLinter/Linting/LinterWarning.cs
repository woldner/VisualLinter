using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter.Linting
{
    internal class LinterWarning
    {
        internal LinterMessage Message { get; }

        internal SnapshotSpan Span { get; }

        internal LinterWarning(SnapshotSpan span, LinterMessage message)
        {
            Message = message;
            Span = span;
        }

        internal static LinterWarning Clone(LinterWarning warning)
        {
            return new LinterWarning(warning.Span, warning.Message);
        }

        internal LinterWarning CloneAndTranslateTo(LinterWarning warning, ITextSnapshot newSnapshot)
        {
            var newSpan = warning.Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            return newSpan.Length == warning.Span.Length
                ? new LinterWarning(newSpan, warning.Message)
                : null;
        }
    }
}