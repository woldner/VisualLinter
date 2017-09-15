using jwldnr.VisualLinter.Enums;
using Newtonsoft.Json;

namespace jwldnr.VisualLinter.Linting
{
    [JsonObject]
    internal class EslintMessage
    {
        [JsonProperty("column")]
        public int Column
        {
            get => _column - 1;
            set => _column = value;
        }

        [JsonProperty("endColumn")]
        public int? EndColumn
        {
            get => _endColumn - 1;
            set => _endColumn = value;
        }

        [JsonProperty("endLine")]
        public int? EndLine
        {
            get => _endLine - 1;
            set => _endLine = value;
        }

        [JsonProperty("fatal")]
        public bool IsFatal { get; set; }

        [JsonProperty("line")]
        public int Line
        {
            get => _line - 1;
            set => _line = value;
        }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonIgnore]
        public MessageRange Range { get; set; }

        [JsonProperty("ruleId")]
        public string RuleId { get; set; }

        [JsonProperty("severity")]
        public RuleSeverity Severity { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        private int _column;
        private int? _endColumn;
        private int? _endLine;
        private int _line;
    }
}