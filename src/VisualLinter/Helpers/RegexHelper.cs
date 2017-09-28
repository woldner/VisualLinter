using System.Text.RegularExpressions;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class RegexHelper
    {
        private const string IgnorePattern = "^[File\\s+ignored]+";

        private const string WordPattern =
            "^[\t ]*$|[^\\s\\/\\\\\\(\\)\"\':,\\.;<>~!@#\\$%\\^&\\*\\|\\+=\\[\\]\\{\\}`\\?\\-…]+";

        internal static Match GetWord(string value)
        {
            return Regex.Match(value, WordPattern);
        }

        internal static bool IgnoreMatch(string value)
        {
            return Regex.Match(value, IgnorePattern)
                .Success;
        }
    }
}