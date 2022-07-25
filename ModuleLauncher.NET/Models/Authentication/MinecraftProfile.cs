using Newtonsoft.Json;

namespace ModuleLauncher.NET.Models.Authentication;

public class MinecraftProfile
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("skins")]
    public List<Skin> skins { get; set; }

    public class Skin
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("varient")]
        public string Varient { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }
    }
}