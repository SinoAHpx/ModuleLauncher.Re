using System;
using ModuleLauncher.Re.Utils.Extensions;
using ModuleLauncher.Re.Utils.JsonConverters;
using Newtonsoft.Json;

namespace ModuleLauncher.Re.Models.Authenticators
{
    [JsonConverter(typeof(JsonPathConverter))]
    public struct AuthenticateResult
    {
        [JsonProperty("selectedProfile.name")]
        public string Name { get; set; }
        
        [JsonProperty("clientToken")]
        public string ClientToken { get; set; }
        
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
        
        [JsonProperty("selectedProfile.id")]
        public string Uuid { get; set; }

        public static implicit operator AuthenticateResult(string s)
        {
            return new AuthenticateResult
            {
                Name = s,
                AccessToken = Guid.NewGuid().ToString("N"),
                ClientToken = Guid.NewGuid().ToString("N"),
                Uuid = Guid.NewGuid().ToString("N"),
            };
        } 
    }
}