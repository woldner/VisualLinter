using System.Text.RegularExpressions;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class RegexHelper
    {
        private const string IndentationPattern = "^\\s+";

        private const string WordPattern =
            "^[\t ]*$|[^\\s\\/\\\\\\(\\)\"\':,\\.;<>~!@#\\$%\\^&\\*\\|\\+=\\[\\]\\{\\}`\\?\\-…]+";

        internal static Match GetIndentation(string value)
        {
            return Regex.Match(value, IndentationPattern);
        }

        internal static Match GetWord(string value)
        {
            return Regex.Match(value, WordPattern);
        }
    }
}