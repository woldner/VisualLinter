using jwldnr.VisualLinter.Enums;
using System.Collections.Generic;

namespace jwldnr.VisualLinter.Linting
{
    public class LinterResult
    {
        public string Source { get; set; }
        public string FilePath { get; set; }
        public int WarningCount { get; set; }
        public int ErrorCount { get; set; }
        public List<LinterMessage> Messages { get; set; }

        public LinterType LinterType { get; set; }

        public LinterResult(EslintResult eslintResult)
        {
            this.Source = eslintResult.Source;
            this.FilePath = eslintResult.FilePath;
            this.WarningCount = eslintResult.WarningCount;
            this.ErrorCount = eslintResult.ErrorCount;
            this.Messages = new List<LinterMessage>();
            foreach (EslintMessage eslintMessage in eslintResult.Messages) this.Messages.Add(new LinterMessage(eslintMessage));

            this.LinterType = LinterType.ESLint;
        }

        public LinterResult(StylelintResult stylelintResult)
        {
            int warningCount = 0;
            int errorCount = 0;
            foreach (StylelintMessage stylelintMessage in stylelintResult.Messages)
            {
                string severity = stylelintMessage.Severity.ToLower();
                if (severity == "warning") warningCount++;
                if (severity == "error") errorCount++;
            }

            this.Source = stylelintResult.Source;
            this.FilePath = string.Empty;
            this.WarningCount = warningCount;
            this.ErrorCount = errorCount;
            this.Messages = new List<LinterMessage>();
            foreach (StylelintMessage stylelintMessage in stylelintResult.Messages) this.Messages.Add(new LinterMessage(stylelintMessage));

            this.LinterType = LinterType.Stylelint;
        }
    }
}
