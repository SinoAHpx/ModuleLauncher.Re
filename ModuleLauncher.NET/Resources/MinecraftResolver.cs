using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Resources;

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

    /// <summary>
    /// Private wrapper of <see cref="RootPath"/>
    /// </summary>
    private DirectoryInfo RootDirectory =>
        new(RootPath.ThrowIfNullOrEmpty<CorruptedStuctureException>("Root path cannot be null"));
    
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
            Assets = RootDirectory.Dive("assets/objects"),
            AssetsIndexes = RootDirectory.Dive("assets/indexes"),
            Jar = RootDirectory.DiveToFile($"versions/{id}/{id}.jar"),
            Json = RootDirectory.DiveToFile($"versions/{id}/{id}.json"),
            VersionRoot = RootDirectory.Dive($"versions/{id}"),
            Natives = RootDirectory.Dive($"versions/{id}/natives")
        };
        var jsonText = tree.Json.ReadAllText();
        var json = JsonConvert.DeserializeObject<MinecraftJson>(jsonText);

        return new MinecraftEntry
        {
            Tree = tree,
            Json = json
        };
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
}