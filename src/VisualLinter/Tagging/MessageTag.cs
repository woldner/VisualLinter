using jwldnr.VisualLinter.Enums;
using jwldnr.VisualLinter.Linting;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;

namespace jwldnr.VisualLinter.Tagging
{
    internal class MessageTag : IErrorTag
    {
        internal MessageTag(LinterMessage message)
        {
            ErrorType = GetErrorType(message);
            ToolTipContent = GetToolTipContent(message.IsFatal, message.Message, message.RuleId);
        }

        public string ErrorType { get; }
        public object ToolTipContent { get; }

        private static string GetErrorType(LinterMessage message)
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
