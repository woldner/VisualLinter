using Newtonsoft.Json;

namespace jwldnr.VisualLinter.Linting
{
    [JsonObject]
    public class StylelintMessage
    {
        [JsonProperty("column")]
        public int Column { get; set; }

        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("rule")]
        public string RuleId { get; set; }
    }
}
