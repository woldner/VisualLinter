using Newtonsoft.Json;

namespace jwldnr.VisualLinter.Linting
{
    [JsonObject]
    public class EslintResult
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("warningCount")]
        public int WarningCount { get; set; }

        [JsonProperty("errorCount")]
        public int ErrorCount { get; set; }

        [JsonProperty("messages")]
        public EslintMessage[] Messages { get; set; }
    }
}
