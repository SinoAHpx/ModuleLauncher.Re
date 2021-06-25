using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ModuleLauncher.Re.Models.Downloaders.Minecraft
{
    public class MinecraftDownloadItem
    {
        /// <summary>
        /// minecraft id
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        
        /// <summary>
        /// minecraft json url
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
        
        /// <summary>
        /// minecraft release time
        /// </summary>
        [JsonProperty("releaseTime")]
        public string ReleaseTime { get; set; }
        
        /// <summary>
        /// minecraft type, Release or Snapshot
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MinecraftDownloadType Type { get; set; }
    }
}