using Newtonsoft.Json;

namespace jwldnr.VisualLinter.Linting
{
    [JsonObject]
    public class StylelintResult
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("errored")]
        public bool Errored { get; set; }

        [JsonProperty("warnings")]
        public StylelintMessage[] Messages { get; set; }
    }
}
