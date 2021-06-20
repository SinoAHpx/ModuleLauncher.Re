using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Models.Locators.Minecraft
{
    public class MinecraftCommonJson
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("libraries")]
        public JArray Libraries { get; set; }
        
        [JsonProperty("mainClass")]
        public string MainClass { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}