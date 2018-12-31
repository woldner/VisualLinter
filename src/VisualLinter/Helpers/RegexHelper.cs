using System.Text.RegularExpressions;

namespace jwldnr.VisualLinter.Helpers
{
    internal static class RegexHelper
    {
        private const string IgnoreFilePattern = "^(File\\s+ignored)+";

        private const string WordPattern =
            "^[\t ]*$|[^\\s\\/\\\\\\(\\)\"\':,\\.;<>~!@#\\$%\\^&\\*\\|\\+=\\[\\]\\{\\}`\\?\\-…]+";

        internal static Match GetWord(string value)
        {
            return Regex.Match(value, WordPattern);
        }

        internal static bool IgnoreFileMatch(string value)
        {
            return ProcessMatch(value, IgnoreFilePattern);
        }

        private static bool ProcessMatch(string value, string pattern)
        {
            var result = Regex
                .Match(value, pattern)
                .Success;

            return result;
        }
    }
}
