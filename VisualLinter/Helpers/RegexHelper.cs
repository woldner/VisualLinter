using System.Text.RegularExpressions;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class RegexHelper
    {
        private const string WhitespacePattern = "^\\s+";

        private const string WordPattern =
            "^[\t ]*$|[^\\s\\/\\\\\\(\\)\"\':,\\.;<>~!@#\\$%\\^&\\*\\|\\+=\\[\\]\\{\\}`\\?\\-…]+";

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