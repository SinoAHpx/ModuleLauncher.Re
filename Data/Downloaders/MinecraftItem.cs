using Newtonsoft.Json;

namespace AHpx.ModuleLauncher.Data.Downloaders
{
    public class MinecraftItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("releaseTime")]
        public string ReleaseTime { get; set; }
        
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}