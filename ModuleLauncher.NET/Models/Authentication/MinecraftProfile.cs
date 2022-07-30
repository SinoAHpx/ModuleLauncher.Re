using ModuleLauncher.NET.Models.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ModuleLauncher.NET.Models.Authentication;

public class MinecraftProfile
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("skins")]
    public List<AccountTesture> skins { get; set; }
    
    [JsonProperty("capes")]
    public List<AccountTesture> Capes {get; set;}

    public class AccountTesture
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("variant")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SkinVariant Variant { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }
    }
}