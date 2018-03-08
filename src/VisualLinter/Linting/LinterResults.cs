using jwldnr.VisualLinter.Enums;
using System.Collections.Generic;

namespace jwldnr.VisualLinter.Linting
{
    public class LinterResults : List<LinterResult>
    {
        public LinterType LinterType { get; set; }

        public LinterResults()
        {
            this.LinterType = LinterType.Unknown;
        }

        public LinterResults(EslintResults eslintResults)
        {
            this.LinterType = LinterType.ESLint;
            foreach (EslintResult eslintResult in eslintResults) this.Add(new LinterResult(eslintResult));
        }

        public LinterResults(StylelintResults stylelintResults)
        {
            this.LinterType = LinterType.Stylelint;
            foreach (StylelintResult stylelintResult in stylelintResults) this.Add(new LinterResult(stylelintResult));
        }
    }
}
