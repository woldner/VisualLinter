using Newtonsoft.Json;
using System.Collections.Generic;

namespace jwldnr.VisualLinter.Linting
{
    [JsonObject]
    internal class EslintResult
    {
        [JsonProperty("errorCount")]
        public int ErrorCount { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("messages")]
        public EslintMessage[] Messages { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("warningCount")]
        public int WarningCount { get; set; }
    }
}