using Newtonsoft.Json;
using System.Collections.Generic;

namespace jwldnr.VisualLinter.Linting
{
    [JsonObject]
    public class LintResult
    {
        [JsonProperty("errorCount")]
        public int ErrorCount { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("messages")]
        public IEnumerable<LintMessage> Messages { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("warningCount")]
        public int WarningCount { get; set; }
    }
}