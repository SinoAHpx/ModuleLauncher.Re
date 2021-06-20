using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json;

namespace ModuleLauncher.Re.Models.Locators.Minecraft
{
    /// <summary>
    /// 1.7.10 -> 1.12.2
    /// </summary>
    [JsonConverter(typeof(JsonPathConverter))]
    public class MinecraftOldJson : MinecraftCommonJson
    {
        [JsonProperty("assets")]
        public string AssetId { get; set; }
        
        [JsonProperty("assetIndex.url")]
        public string AssetIndexUrl { get; set; }
        
        [JsonProperty("minecraftArguments")]
        public string MinecraftArguments { get; set; }
    }
}