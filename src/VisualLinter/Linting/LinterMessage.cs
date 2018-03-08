using jwldnr.VisualLinter.Enums;

namespace jwldnr.VisualLinter.Linting
{
    public class LinterMessage
    {
        public string Source { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public int? EndLine { get; set; }
        public int? EndColumn { get; set; }
        public bool IsFatal { get; set; }
        public string Message { get; set; }
        public string RuleId { get; set; }
        public RuleSeverity Severity { get; set; }

        public MessageRange Range { get; set; }
        public LinterType LinterType { get; set; }

        public LinterMessage(EslintMessage eslintMessage)
        {
            this.Source = eslintMessage.Source;
            this.Line = eslintMessage.Line;
            this.Column = eslintMessage.Column;
            this.EndLine = eslintMessage.EndLine != null ? eslintMessage.EndLine : null;
            this.EndColumn = eslintMessage.EndColumn != null ? eslintMessage.EndColumn : null;
            this.IsFatal = eslintMessage.IsFatal;
            this.Message = eslintMessage.Message;
            this.RuleId = eslintMessage.RuleId;
            this.Severity = RuleSeverity.Off;
            int severity = eslintMessage.Severity;
            if (severity == 1) this.Severity = RuleSeverity.Warn;
            if (severity == 2) this.Severity = RuleSeverity.Error;

            if (this.Line >= 1) this.Line--;
            if (this.Column >= 1) this.Column--;
            if (this.EndLine.HasValue && this.EndLine >= 1) this.EndLine--;
            if (this.EndColumn.HasValue && this.EndColumn >= 1) this.EndColumn--;

            this.Range = null;
            this.LinterType = LinterType.ESLint;
        }

        public LinterMessage(StylelintMessage stylelintMessage)
        {
            this.Source = string.Empty;
            this.Line = stylelintMessage.Line;
            this.Column = stylelintMessage.Column;
            this.EndLine = stylelintMessage.Line;
            this.EndColumn = stylelintMessage.Column;
            this.IsFatal = false;
            this.Message = stylelintMessage.Text;
            this.RuleId = stylelintMessage.RuleId;
            this.Severity = RuleSeverity.Off;
            string severity = stylelintMessage.Severity.ToLower();
            if (severity == "warning") this.Severity = RuleSeverity.Warn;
            if (severity == "error") this.Severity = RuleSeverity.Error;

            if (this.Line >= 1) this.Line--;
            if (this.Column >= 1) this.Column--;
            if (this.EndLine.HasValue && this.EndLine >= 1) this.EndLine--;
            if (this.EndColumn.HasValue && this.EndColumn >= 1) this.EndColumn--;

            this.Range = null;
            this.LinterType = LinterType.Stylelint;
        }
    }
}
