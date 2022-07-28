using System.ComponentModel;
using System.Runtime.Serialization;
using Manganese.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.NET.Models.Resources;

[JsonConverter(typeof(JsonPathConverter))]
public class MinecraftJson
{
    /// <summary>
    /// Minecraft id
    /// <example>Vanilla: 1.16.1, Loaders: 1.16.5-OptiFine_HD_U_G8, 1.18.2-forge-40.1.68</example>
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Libraries, may be incomplete
    /// </summary>
    [JsonProperty("libraries")]
    public JArray? Libraries { get; set; }

    /// <summary>
    /// <example>Vanilla: net.minecraft.client.main.Main, Loaders: cpw.mods.bootstraplauncher.BootstrapLauncher, net.minecraft.launchwrapper.Launch</example>
    /// </summary>
    [JsonProperty("mainClass")]
    public string? MainClass { get; set; }

    /// <summary>
    /// Type of Minecraft, can be release, snapshot, old_alpha and old_beta
    /// </summary>
    [JsonProperty("type")]
    [JsonConverter(typeof(StringEnumConverter))]
    public MinecraftJsonType? Type { get; set; }

    [JsonProperty("assets")] public string? AssetId { get; set; }

    [JsonProperty("assetIndex.url")] public string? AssetIndexUrl { get; set; }
    
    [JsonProperty("javaVersion.majorVersion")]
    public int? JavaVersion {get; set;}

    [JsonProperty("arguments")] public JToken? Arguments { get; set; }

    [JsonProperty("minecraftArguments")] public string? MinecraftArguments { get; set; }

    [JsonProperty("inheritsFrom")] public string? InheritsFrom { get; set; }
}

public enum MinecraftJsonType
{
    [EnumMember(Value = "old_alpha")]
    [Description("old_alpha")]
    OldAlpha,
    
    [EnumMember(Value = "old_beta")]
    [Description("old_beta")]
    OldBeta,
    
    [EnumMember(Value = "release")]
    [Description("release")]
    Release,
    
    [EnumMember(Value = "snapshot")]
    [Description("snapshot")]
    Snapshot
}