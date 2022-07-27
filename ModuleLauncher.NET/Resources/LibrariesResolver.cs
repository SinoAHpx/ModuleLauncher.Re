using Manganese.Data;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.NET.Resources;

public class LibrariesResolver
{
    public MinecraftEntry Minecraft { get; set; }

    public List<LibraryEntry> GetLibraries()
    {
        var libraries = new List<LibraryEntry>();

        var rawLibraries = Minecraft.Json.Libraries
            .ThrowIfNull(new ErrorParsingLibraryException("Corrupted json"));

        foreach (var rawLibrary in rawLibraries)
        {
            if (!IsAvailableLibrary(rawLibrary))
                continue;

            var rawLibraryObj = rawLibrary.ToObject<JObject>()
                .ThrowIfNull(new ErrorParsingLibraryException($"Json file corrupted: {rawLibrary}"));
            
            if (rawLibraryObj.ContainsKey("natives"))
                libraries.Add(ProcessNative(rawLibrary));
            else
                libraries.Add(ProcessLibrary(rawLibrary));
        }

        return libraries.DistinctBy(e => e.Name).ToList();
    }

    internal LibraryEntry ProcessLibrary(JToken rawLib)
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

    internal LibraryEntry ProcessNative(JToken rawNative)
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
    internal bool IsAvailableLibrary(JToken rawLib)
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