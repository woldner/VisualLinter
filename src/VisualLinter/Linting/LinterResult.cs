using Newtonsoft.Json;

namespace jwldnr.VisualLinter.Linting
{
    [JsonObject]
    public class LinterResult
    {
        [JsonProperty("errorCount")] public int ErrorCount { get; set; }

        [JsonProperty("filePath")] public string FilePath { get; set; }

        [JsonProperty("messages")] public LinterMessage[] Messages { get; set; }

        [JsonProperty("source")] public string Source { get; set; }

        [JsonProperty("warningCount")] public int WarningCount { get; set; }
    }
}
