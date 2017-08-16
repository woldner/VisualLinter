using Newtonsoft.Json;

namespace jwldnr.VisualLinter.Linting
{
    [JsonObject]
    internal class LinterMessage
    {
        [JsonProperty("column")]
        public int Column { get; set; }

        [JsonProperty("endColumn")]
        public int? EndColumn { get; set; }

        [JsonProperty("endLine")]
        public int? EndLine { get; set; }

        [JsonProperty("fatal")]
        public bool IsFatal { get; set; }

        [JsonProperty("line")]
        public int Line { get; set; }

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
    }
}