using jwldnr.VisualLinter.Enums;
using jwldnr.VisualLinter.Linting;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;

namespace jwldnr.VisualLinter.Tagging
{
    internal class LinterTag : IErrorTag
    {
        public string ErrorType { get; }
        public object ToolTipContent { get; }

        internal LinterTag(EslintMessage message)
        {
            ErrorType = GetErrorType(message);
            ToolTipContent = GetToolTipContent(message.IsFatal, message.Message, message.RuleId);
        }

        private static string GetErrorType(EslintMessage message)
        {
            if (message.IsFatal)
                return PredefinedErrorTypeNames.SyntaxError;

            return RuleSeverity.Error == message.Severity
                ? PredefinedErrorTypeNames.SyntaxError
                : PredefinedErrorTypeNames.Warning;
        }

        private static object GetToolTipContent(bool isFatal, string message, string ruleId)
        {
            return isFatal
                ? message
                : $"{message} ({ruleId})";
        }
    }
}
