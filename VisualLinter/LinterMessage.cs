using Newtonsoft.Json;

namespace jwldnr.VisualLinter
{
    [JsonObject]
    internal class LinterMessage
    {
        [JsonProperty("column")]
        internal int Column { get; set; }

        [JsonProperty("endColumn")]
        internal int? EndColumn { get; set; }

        [JsonProperty("endLine")]
        internal int? EndLine { get; set; }

        [JsonProperty("fatal")]
        internal bool IsFatal { get; set; }

        [JsonProperty("line")]
        internal int Line { get; set; }

        [JsonProperty("message")]
        internal string Message { get; set; }

        [JsonIgnore]
        internal TextRange Range { get; set; }

        [JsonProperty("ruleId")]
        internal string RuleId { get; set; }

        [JsonProperty("severity")]
        internal int Severity { get; set; }

        [JsonProperty("source")]
        internal string Source { get; set; }
    }
}