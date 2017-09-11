using Microsoft.VisualStudio.Text;

namespace jwldnr.VisualLinter.Linting
{
    internal class LinterMarker
    {
        internal LinterMessage Message { get; }

        internal SnapshotSpan Span { get; }

        internal LinterMarker(SnapshotSpan span, LinterMessage message)
        {
            Message = message;
            Span = span;
        }

        internal static LinterMarker Clone(LinterMarker marker)
        {
            return new LinterMarker(marker.Span, marker.Message);
        }

        internal LinterMarker CloneAndTranslateTo(LinterMarker marker, ITextSnapshot newSnapshot)
        {
            var newSpan = marker.Span.TranslateTo(newSnapshot, SpanTrackingMode.EdgeExclusive);

            return newSpan.Length == marker.Span.Length
                ? new LinterMarker(newSpan, marker.Message)
                : null;
        }
    }
}