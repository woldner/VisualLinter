namespace jwldnr.VisualLinter.Helpers
{
    internal static class StringExtensions
    {
        public static string NullIfEmpty(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
