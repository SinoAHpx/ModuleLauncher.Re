using Manganese.Data;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Resources;

/// <summary>
/// Minecraft resolver, could be convert mutually with string
/// </summary>
public class MinecraftResolver
{
    /// <summary>
    /// .minecraft path or equivalent path 
    /// </summary>
    public string? RootPath { get; set; }

    public MinecraftResolver(string? rootPath = null)
    {
        RootPath = rootPath;
    }

    public static implicit operator MinecraftResolver(string minecraftRootPath)
    {
        return new MinecraftResolver(minecraftRootPath);
    }

    public static implicit operator string(MinecraftResolver resolver)
    {
        return resolver.RootPath.ThrowIfNullOrEmpty<NullReferenceException>("Root path of resolver could not be null");
    }

    /// <summary>
    /// Private wrapper of <see cref="RootPath"/>
    /// </summary>
    private DirectoryInfo RootDirectory =>
        new(RootPath.ThrowCorruptedIfNull());
    
    /// <summary>
    /// Retrieve Minecraft entry by Minecraft id
    /// </summary>
    /// <param name="id">e.g. 1.16.1</param>
    /// <returns></returns>
    /// <exception cref="CorruptedStuctureException"></exception>
    public MinecraftEntry GetMinecraft(string id)
    {
        if (!RootDirectory.Exists)
            throw new CorruptedStuctureException("Minecraft path does not exist");

        var tree = new MinecraftTree
        {
            Root = RootDirectory,
            Versions = RootDirectory.Dive("versions"),
            Saves = RootDirectory.Dive("saves"),
            Mods = RootDirectory.Dive("mods"),
            ResourcesPacks = RootDirectory.Dive("resourcepacks"),
            TexturePacks = RootDirectory.Dive("texturepacks"),
            Libraries = RootDirectory.Dive("libraries"),
            Assets = RootDirectory.Dive("assets"),
            AssetsIndexes = RootDirectory.Dive("assets/indexes"),
            Jar = RootDirectory.DiveToFile($"versions/{id}/{id}.jar"),
            Json = RootDirectory.DiveToFile($"versions/{id}/{id}.json"),
            VersionRoot = RootDirectory.Dive($"versions/{id}"),
            Natives = RootDirectory.Dive($"versions/{id}/natives")
        };

        if (!tree.VersionRoot.Exists)
            throw new CorruptedStuctureException("Minecraft path does not exist");

        var jsonText = tree.Json.ReadAllText();
        var json = JsonConvert.DeserializeObject<MinecraftJson>(jsonText).ThrowCorruptedIfNull();

        var entry = new MinecraftEntry
        {
            Tree = tree,
            Json = json
        };

        if (entry.Json.AssetId == "legacy" || entry.Json.AssetId == "pre-1.6")
            entry.Tree.Assets = entry.Tree.Assets.Dive("virtual/legacy");
        
        if (entry.GetMinecraftType() != MinecraftType.Vanilla && entry.Json.InheritsFrom.IsNullOrEmpty())
            entry.Tree.Assets = entry.Tree.Assets.Dive("virtual/legacy");
        

        return entry;
    }

    /// <summary>
    /// Get all minecraft entries in the root directory
    /// </summary>
    /// <returns></returns>
    public List<MinecraftEntry> GetMinecrafts()
    {
        var re = new List<MinecraftEntry>();
        var dirs = RootDirectory.Dive("versions").GetDirectories();
        foreach (var dir in dirs)
        {
            re.Add(GetMinecraft(dir.Name));
        }

        return re;
    }

    /// <summary>
    /// Initialize MinecraftResolver by minecraft entry
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static MinecraftResolver Of(MinecraftEntry minecraftEntry)
    {
        return new MinecraftResolver(minecraftEntry.Tree.Root.FullName);
    }
}