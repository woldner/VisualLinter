using Microsoft.VisualStudio.Text;
using System;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class TextSnapshotHelper
    {
        public static SnapshotPoint GetPointInLine(this ITextSnapshot snapshot, int line, int column)
        {
            var snapshotLine = snapshot.GetLineFromLineNumber(line);
            return snapshotLine.Start.Add(column);
        }

        internal static bool ValidatePoint(this ITextSnapshot snapshot, int line, int column)
        {
            try
            {
                snapshot.GetPointInLine(line, column);
                return true;
            }
            catch (Exception)
            {
                OutputWindowHelper.WriteLine($"error: {line}:{column} isn't a valid point!");
            }

            return false;
        }
    }
}