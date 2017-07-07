using Newtonsoft.Json;
using System.Collections.Generic;

namespace jwldnr.VisualLinter
{
    [JsonObject]
    internal class LinterResult
    {
        [JsonProperty("errorCount")]
        internal int ErrorCount { get; set; }

        [JsonProperty("filePath")]
        internal string FilePath { get; set; }

        [JsonProperty("messages")]
        internal IEnumerable<LinterMessage> Messages { get; set; }

        [JsonProperty("source")]
        internal string Source { get; set; }

        [JsonProperty("warningCount")]
        internal int WarningCount { get; set; }
    }
}