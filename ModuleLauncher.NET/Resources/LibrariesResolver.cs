using Manganese.Data;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.NET.Resources;

public class LibrariesResolver
{
    /// <summary>
    /// Minecraft resolver, you'll need to provide this only when you need to get libraries by minecraft id
    /// </summary>
    public MinecraftResolver? MinecraftResolver { get; set; }

    /// <summary>
    /// Construct resolver via minecraft resolver
    /// </summary>
    /// <param name="minecraftResolver"></param>
    public LibrariesResolver(MinecraftResolver? minecraftResolver)
    {
        MinecraftResolver = minecraftResolver;
    }

    /// <summary>
    /// Construct libraries resolver via minecraft root path
    /// </summary>
    /// <param name="minecraftRootPath"></param>
    public LibrariesResolver(string? minecraftRootPath)
    {
        MinecraftResolver =
            minecraftRootPath.ThrowIfNullOrEmpty<NullReferenceException>("Root path of resolver could not be null");
    }

    /// <summary>
    /// Just an empty constructor
    /// </summary>
    public LibrariesResolver()
    {
    }

    /// <summary>
    /// Get minecraft libraries by id
    /// <remarks>Need to provide MinecraftSolver if you need to get libraries by minecraft id</remarks>
    /// </summary>
    /// <param name="minecraftId"></param>
    /// <returns></returns>
    public List<LibraryEntry> GetLibraries(string minecraftId)
    {
        var minecraftEntry = MinecraftResolver
            .ThrowIfNull(
                new InvalidOperationException(
                    "Need to provide MinecraftSolver if you need to get libraries by minecraft id"))
            .GetMinecraft(minecraftId)
            .ThrowIfNull(new InvalidOperationException($"Specify Minecraft {minecraftId} does not exist"));

        return GetLibraries(minecraftEntry);
    }

    /// <summary>
    /// Get 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static List<LibraryEntry> GetLibraries(MinecraftEntry minecraftEntry)
    {
        var rawLibraries = minecraftEntry.Json.Libraries
            .ThrowIfNull(new ErrorParsingLibraryException("Corrupted json"));

        var libraries = new List<LibraryEntry>();
        
        foreach (var rawLibrary in rawLibraries)
        {
            if (!IsAvailableLibrary(rawLibrary))
                continue;

            var rawLibraryObj = rawLibrary.ToObject<JObject>()
                .ThrowIfNull(new ErrorParsingLibraryException($"Json file corrupted: {rawLibrary}"));

            var toAdd = rawLibraryObj.ContainsKey("natives")
                ? ProcessNative(rawLibrary)
                : ProcessLibrary(rawLibrary);

            toAdd.Type = minecraftEntry.GetMinecraftType();
            
            libraries.Add(toAdd);
        }
        
        if (minecraftEntry.GetMinecraftType() != MinecraftType.Vanilla)
        {
            // note: in the version which don't really have a "inheritFrom" key,
            // we don't have to give them additional libraries neither
            if (!minecraftEntry.Json.InheritsFrom.IsNullOrEmpty())
            {
                var resolver = new MinecraftResolver(minecraftEntry.Tree.Root.FullName);
                var inheritMinecraft = resolver.GetMinecraft(minecraftEntry.Json.InheritsFrom)
                    .ThrowIfNull(new InvalidOperationException($"Specify Minecraft {minecraftEntry.Json.InheritsFrom} does not exist"));

                var inheritLibraries = GetLibraries(inheritMinecraft);
                libraries.AddRange(inheritLibraries);
            }
        }

        return libraries.DistinctBy(e => e.Name).ToList();
    }

    private static LibraryEntry ProcessLibrary(JToken rawLib)
    {
        var rawName = rawLib.Fetch("name")
            .ThrowIfNullOrEmpty<ErrorParsingLibraryException>($"Cannot find name in {rawLib}");

        var processName = rawName.ResolveRawName();

        var libEntry = new LibraryEntry
        {
            Name = processName.Name,
            IsNative = false,
            RelativeUrl = processName.RelativeUrl,
            RelativePath = processName.RelativePath
        };

        return libEntry;
    }

    private static LibraryEntry ProcessNative(JToken rawNative)
    {
        var rawObj = rawNative.ToObject<JObject>()
            .ThrowIfNull(new ErrorParsingLibraryException("Corrupted json file"));

        var rawName = rawObj.Fetch("name")
            .ThrowIfNullOrEmpty<ErrorParsingLibraryException>("Corrupted json file");

        var libEntry = new LibraryEntry { IsNative = true };

        if (rawObj.ContainsKey("natives"))
        {
            var suffix = rawObj.Fetch($"natives.{CommonUtils.CurrentSystemName}")
                .ThrowIfNullOrEmpty<ErrorParsingLibraryException>("Corrupted json file");
            suffix = suffix.Replace("${arch}", CommonUtils.SystemArch);

            libEntry.Name = rawName.ResolveRawName(suffix).Name;
        }
        else
        {
            libEntry.Name = rawName.ResolveRawName().Name;
        }

        return libEntry;
    }

    /// <summary>
    /// Checking rules
    /// </summary>
    /// <returns></returns>
    private static bool IsAvailableLibrary(JToken rawLib)
    {
        try
        {
            var systemName = CommonUtils.CurrentSystemName;
            var rules = rawLib.Fetch("rules")
                .ThrowIfNullOrEmpty<ErrorParsingLibraryException>("114514")
                .ToJArray();

            if (rules.Any(token => token.Fetch("action") == "disallow" && token.Fetch("os.name") == systemName))
                return false;

            return rules.Any(token => token.Fetch("action") == "allow" && token.Fetch("os.name") == systemName) ||
                   rules.Any(token => token.Fetch("action") == "allow" && token.Fetch("os.name").IsNullOrEmpty());
        }
        catch (ErrorParsingLibraryException)
        {
            // in case of no rules property
            return true;
        }
    }
}