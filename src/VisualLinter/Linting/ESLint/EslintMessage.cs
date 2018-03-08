using Newtonsoft.Json;

namespace jwldnr.VisualLinter.Linting
{
    [JsonObject]
    public class EslintMessage
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("column")]
        public int Column { get; set; }

        [JsonProperty("endColumn")]
        public int? EndColumn { get; set; }

        [JsonProperty("endLine")]
        public int? EndLine { get; set; }

        [JsonProperty("fatal")]
        public bool IsFatal { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("ruleId")]
        public string RuleId { get; set; }

        [JsonProperty("severity")]
        public int Severity { get; set; }
    }
}
