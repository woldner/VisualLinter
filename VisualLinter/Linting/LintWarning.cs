using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter.Linting
{
    internal class LintWarning
    {
        internal LintMessage Message { get; }

        internal SnapshotSpan Span { get; }

        internal LintWarning(SnapshotSpan span, LintMessage message)
        {
            Message = message;
            Span = span;
        }

        internal static LintWarning Clone(LintWarning warning)
        {
            return new LintWarning(warning.Span, warning.Message);
        }

        internal LintWarning CloneAndTranslateTo(LintWarning warning, ITextSnapshot newSnapshot)
        {
            var newSpan = warning.Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            return newSpan.Length == warning.Span.Length
                ? new LintWarning(newSpan, warning.Message)
                : null;
        }
    }
}