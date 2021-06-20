using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Models.Locators.Minecraft
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class MinecraftJson
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("libraries")]
        public JArray Libraries { get; set; }
        
        [JsonProperty("mainClass")]
        public string MainClass { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("assets")]
        public string AssetId { get; set; }
        
        [JsonProperty("assetIndex.url")]
        public string AssetIndexUrl { get; set; }
        
        [JsonProperty("arguments")]
        public JToken Arguments { get; set; }

        [JsonProperty("minecraftArguments")]
        public string MinecraftArguments { get; set; }

        [JsonProperty("inheritsFrom")]
        public string InheritsFrom { get; set; }
    }
}