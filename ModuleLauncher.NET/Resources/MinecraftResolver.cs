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
    private string _rootPath;

    /// <summary>
    /// .minecraft path or equivalent path 
    /// </summary>
    public string RootPath
    {
        get => _rootPath;
        set => _rootPath = value.ThrowIfNullOrEmpty<NullReferenceException>("Root path of resolver could not be null");
    }

    public MinecraftResolver(string rootPath)
    {
        RootPath = rootPath;
    }

    public MinecraftResolver()
    {
    }

    public static implicit operator MinecraftResolver(string minecraftRootPath)
    {
        return new MinecraftResolver(minecraftRootPath);
    }

    public static implicit operator string(MinecraftResolver resolver)
    {
        return resolver.RootPath;
    }

    /// <summary>
    /// Private wrapper of <see cref="RootPath"/>
    /// </summary>
    private DirectoryInfo RootDirectory => new(RootPath);

    /// <summary>
    /// Retrieve Minecraft entry by Minecraft id
    /// </summary>
    /// <param name="id">e.g. 1.16.1</param>
    /// <param name="workingDirectory">Working directory for single Minecraft, usually an absolute path</param>
    /// <returns></returns>
    /// <exception cref="CorruptedStuctureException"></exception>
    public MinecraftEntry GetMinecraft(string id, string? workingDirectory = null)
    {
        if (!RootDirectory.Exists)
            throw new CorruptedStuctureException("Minecraft path does not exist");

        var tree = new MinecraftTree
        {
            Root = RootDirectory,
            WorkingDirectory = RootDirectory,
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

        if (!workingDirectory.IsNullOrEmpty()) tree.WorkingDirectory = new DirectoryInfo(workingDirectory);

        if (!tree.VersionRoot.Exists)
            throw new CorruptedStuctureException("Minecraft path does not exist");

        var jsonText = tree.Json.ReadAllText();
        var json = JsonConvert.DeserializeObject<MinecraftJson>(jsonText).ThrowCorruptedIfNull();
        json.Raw = jsonText.ToJObject();

        var entry = new MinecraftEntry
        {
            Tree = tree,
            Json = json
        };

        if (entry.Json.AssetId is "legacy" or "pre-1.6" ||
            (entry.GetMinecraftType() != MinecraftType.Vanilla && !entry.HasInheritSource()))
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
        foreach (var dir in dirs) re.Add(GetMinecraft(dir.Name));

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