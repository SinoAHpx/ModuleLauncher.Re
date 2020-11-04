using System.Collections.Generic;
using Newtonsoft.Json;

namespace AHpx.ModuleLauncher.Data.Authentication
{
    public class ExternalAuthenticateMeta
    {
        [JsonProperty("skinDomains")]
        public IEnumerable<string> SkinDomains { get; set; }
        
        [JsonProperty("signaturePublickey")]
        public string SignaturePublickey { get; set; }
        
        [JsonProperty("meta")]
        public MetaData Meta { get; set; }

        public class MetaData
        {
            [JsonProperty("feature.non_email_login")]
            public string NonEmailLogin { get; set; }
        
            [JsonProperty("implementationName")]
            public string ImplementationName { get; set; }
        
            [JsonProperty("serverName")]
            public string ServerName { get; set; }
            
            [JsonProperty("implementationVersion")]
            public string ImplementationVersion { get; set; }
        }
    }
}