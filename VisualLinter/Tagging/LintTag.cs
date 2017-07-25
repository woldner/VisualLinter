using jwldnr.VisualLinter.Linting;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;

namespace jwldnr.VisualLinter.Tagging
{
    internal class LintTag : IErrorTag
    {
        public string ErrorType { get; }

        public object ToolTipContent { get; }

        internal LintTag(LintMessage message)
        {
            ErrorType = GetErrorType(message.IsFatal);
            ToolTipContent = GetToolTipContent(message.Message, message.RuleId);
        }

        private static string GetErrorType(bool isFatal)
        {
            return isFatal
                ? PredefinedErrorTypeNames.SyntaxError
                : PredefinedErrorTypeNames.Warning;
        }

        private static object GetToolTipContent(string message, string ruleId)
        {
            return null != ruleId
                ? $"{message} ({ruleId})"
                : message;
        }
    }
}