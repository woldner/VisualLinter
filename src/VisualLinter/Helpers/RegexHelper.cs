using System.Text.RegularExpressions;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class RegexHelper
    {
        private const string IgnorePattern = "^[File\\s+ignored]+";

        private const string WhitespacePattern = "^\\s+";

        private const string WordPattern =
            "^[\t ]*$|[^\\s\\/\\\\\\(\\)\"\':,\\.;<>~!@#\\$%\\^&\\*\\|\\+=\\[\\]\\{\\}`\\?\\-…]+";

        internal static bool IgnoreMatch(string value)
        {
            return Regex.Match(value, IgnorePattern)
                .Success;
        }

        internal static Match GetIndentation(string value)
        {
            return Regex.Match(value, WhitespacePattern);
        }

        internal static Match GetWord(string value)
        {
            return Regex.Match(value, WordPattern);
        }
    }
}