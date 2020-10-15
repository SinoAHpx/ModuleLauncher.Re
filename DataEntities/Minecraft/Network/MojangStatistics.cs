using Newtonsoft.Json;

namespace ModuleLauncher.Re.DataEntities.Minecraft.Network
{
    public class MojangStatistics
    {
        [JsonProperty("total")] public string Total { get; set; }
        [JsonProperty("last24h")] public string Yesterday { get; set; }

        [JsonProperty("saleVelocityPerSeconds")]
        public string VelocityPerSeconds { get; set; }
    }
}