using Newtonsoft.Json.Linq;

namespace ModuleLauncher.NET.Models.Resources;

public class AssetEntry
{
    /// <summary>
    /// Asset file
    /// </summary>
    public FileInfo File { get; set; }

    /// <summary>
    /// Asset's relative url
    /// </summary>
    public string RelativeUrl { get; set; }

    /// <summary>
    /// Last version that using legacy assets scheme is 1.7.2(release)/13w48b(snapshot)
    /// </summary>
    public bool IsLegacy { get; set; }

    /// <summary>
    /// Assets hash
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// For versions prior 1.6
    /// </summary>
    public bool MapToResource { get; set; }

    /// <summary>
    /// Raw json key-value entry
    /// </summary>
    public KeyValuePair<string, JToken> Raw { get; set; }
}