using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json;

namespace ModuleLauncher.Re.Models.Authenticators
{
    [JsonConverter(typeof(JsonPathConverter))]
    public class AuthenticateResult
    {
        [JsonProperty("selectedProfile.name")]
        public string Name { get; set; }
        
        [JsonProperty("clientToken")]
        public string ClientToken { get; set; }
        
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
        
        [JsonProperty("selectedProfile.id")]
        public string Uuid { get; set; }
    }
}