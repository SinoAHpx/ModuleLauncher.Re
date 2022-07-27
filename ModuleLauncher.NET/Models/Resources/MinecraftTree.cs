using Manganese.Text;

namespace ModuleLauncher.NET.Models.Resources;

/// <summary>
/// Tree structure of specified Minecraft version
/// </summary>
public class MinecraftTree
{
    /// <summary>
    /// .minecraft
    /// </summary>
    public DirectoryInfo Root { get; set; }

    /// <summary>
    /// .minecraft/versions
    /// </summary>
    public DirectoryInfo Versions { get; set; }

    /// <summary>
    /// .minecraft/saves
    /// </summary>
    public DirectoryInfo Saves { get; set; }

    /// <summary>
    /// .minecraft/mods
    /// </summary>
    public DirectoryInfo Mods { get; set; }

    /// <summary>
    /// .minecraft/resourcepacks
    /// </summary>
    public DirectoryInfo ResourcesPacks { get; set; }

    /// <summary>
    /// .minecraft/texturepacks
    /// <remarks>Only for old version minecraft</remarks>
    /// </summary>
    public DirectoryInfo TexturePacks { get; set; }

    /// <summary>
    /// .minecraft/libraries
    /// </summary>
    public DirectoryInfo Libraries { get; set; }

    /// <summary>
    /// .minecraft/assets/objects
    /// </summary>
    public DirectoryInfo Assets { get; set; }

    /// <summary>
    /// .minecraft/assets/indexes
    /// </summary>
    public DirectoryInfo AssetsIndexes { get; set; }

    /// <summary>
    /// .minecraft/versions/%ver%/%ver%.jar
    /// </summary>
    public FileInfo Jar { get; set; }

    /// <summary>
    /// .minecraft/versions/%ver%/%ver%.json
    /// </summary>
    public FileInfo Json { get; set; }

    /// <summary>
    /// .minecraft/versions/%ver%
    /// </summary>
    public DirectoryInfo VersionRoot { get; set; }

    /// <summary>
    /// .minecraft/versions/%ver%/natives
    /// </summary>
    public DirectoryInfo Natives { get; set; }

    public override string ToString()
    {
        return new
        {
            Root = Root.FullName,
            Versions = Versions.FullName,
            Saves = Saves.FullName,
            Mods = Mods.FullName,
            ResourcesPacks = ResourcesPacks.FullName,
            TexturePacks = TexturePacks.FullName,
            Libraries = Libraries.FullName,
            Assets = Assets.FullName,
            AssetsIndexes = AssetsIndexes.FullName,
            Jar = Jar.FullName,
            Json = Json.FullName,
            VersionRoot = VersionRoot.FullName,
            Natives = Natives.FullName
        }.ToJsonString();
    }
}