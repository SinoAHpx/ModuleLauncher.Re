namespace ModuleLauncher.NET.Models.Resources;

/// <summary>
/// A single library
/// </summary>
public class LibraryEntry
{
    /// <summary>
    /// <example>patchy-1.3.9.jar</example>
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// <example>com/mojang/patchy/1.3.9/patchy-1.3.9.jar</example>
    /// </summary>
    public string RelativeUrl { get; set; }

    /// <summary>
    /// <example>On Windows: com\mojang\patchy\1.3.9\patchy-1.3.9.jar, on other os this could be same as <see cref="RelativeUrl"/></example>
    /// </summary>
    public string RelativePath { get; set; }

    /// <summary>
    /// Note: native libraries is to extract
    /// </summary>
    public bool IsNative { get; set; }

    /// <summary>
    /// What kind of json file this entry in
    /// </summary>
    public MinecraftType Type { get; set; }
}