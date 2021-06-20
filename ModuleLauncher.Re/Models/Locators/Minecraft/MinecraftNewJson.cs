using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Models.Locators.Minecraft
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class MinecraftNewJson : MinecraftCommonJson
    {
        [JsonProperty("assets")]
        public string AssetId { get; set; }
        
        [JsonProperty("assetIndex.url")]
        public string AssetIndexUrl { get; set; }
        
        [JsonProperty("arguments")]
        public JToken MinecraftArguments { get; set; }
    }
}