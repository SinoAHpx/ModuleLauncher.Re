using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ModuleLauncher.NET.Models.Resources;

public class RemoteMinecraftEntry
{
    /// <summary>
    /// minecraft id
    /// </summary>
    [JsonProperty("id")]
    public string Id { get; set; }

    /// <summary>
    /// minecraft json url
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }

    /// <summary>
    /// minecraft release time
    /// </summary>
    [JsonProperty("releaseTime")]
    public DateTime ReleaseTime { get; set; }

    /// <summary>
    /// Sha1 value
    /// </summary>
    [JsonProperty("sha1")]
    public string Sha1 { get; set; }

    /// <summary>
    /// minecraft type, Release or Snapshot
    /// </summary>
    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public MinecraftJsonType Type { get; set; }
}