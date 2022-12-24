using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using Manganese.Text;
using ModuleLauncher.NET.Mods.Models.Exceptions;
using ModuleLauncher.NET.Mods.Models.Utils;
using Tommy;

namespace ModuleLauncher.NET.Mods.Utilities;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class ModUtils
{
    public static async Task<ModInfo> GetModInfoAsync(string modPath)
    {
        return await GetModInfoAsync(new FileInfo(modPath));
    }

    public static async Task<ModInfo> GetModInfoAsync(FileInfo mod)
    {
        var archive = ZipFile.OpenRead(mod.FullName);
        if (archive.Entries.Any(e => e.Name == "mcmod.info"))
            return await GetModInfoLegacyForgeAsync(archive);
        if (archive.Entries.Any(e => e.Name == "mods.toml"))
            return await GetModInfoForgeAsync(archive);
        if (archive.Entries.Any(e => e.Name is "fabric.mod.json" or "quilt.mod.json"))
            return await GetModInfoFabricAsync(archive);

        throw new UnknownModException("Unknown mod or mod is corrupted");
    }

    private static async Task<ModInfo> GetModInfoLegacyForgeAsync(ZipArchive archive)
    {
        var zipEntry = archive.GetEntry("mcmod.info")!;
        await using var stream = zipEntry.Open();
        var raw = await new StreamReader(stream).ReadToEndAsync();
        var content = raw.ToJArray().First!;
        var re = new ForgeModInfo
        {
            Name = content.Fetch("name"),
            Id = content.Fetch("modid"),
            Description = content.Fetch("description"),
            MinecraftVersion = content.Fetch("mcversion"),
            Url = content.Fetch("url"),
            Version = content.Fetch("version"),
            Authors = content.FetchJToken("authorList")?.Select(t => t.ToString()).ToList(),
        };

        return re;
    }

    private static async Task<ModInfo> GetModInfoForgeAsync(ZipArchive archive)
    {
        var zipEntry = archive.GetEntry("META-INF/mods.toml")!;
        await using var stream = zipEntry.Open();
        using var raw = new StreamReader(stream);

        var tomTable = TOML.Parse(raw);
        var modNode = tomTable["mods"].AsArray[0];
        var re = new ForgeModInfo
        {
            Id = modNode["modId"],
            Version = modNode["version"].HasValue ? modNode["version"] : tomTable["version"],
            Name = modNode["displayName"].HasValue ? modNode["displayName"] : tomTable["displayName"],
            Url = modNode["displayURL"].HasValue ? modNode["displayURL"] : tomTable["displayURL"],
            License = tomTable["license"].HasValue ? tomTable["license"] : modNode["license"],
            Description = modNode["description"].HasValue ? modNode["description"] : tomTable["descrption"],
        };

        if (modNode["authors"].HasValue)
            re.Authors = modNode["authors"].IsArray
                ? modNode["authors"].AsArray.RawArray.Select(x => x.ToString()).ToList()!
                : new List<string> { modNode["authors"] };
        else
            re.Authors = tomTable["authors"].IsArray
                ? tomTable["authors"].AsArray.RawArray.Select(x => x.ToString()).ToList()!
                : new List<string> { tomTable["authors"] };

        return re;
    }

    private static async Task<ModInfo> GetModInfoFabricAsync(ZipArchive archive)
    {
        var zipEntry = archive.GetEntry("quilt.mod.json") ?? archive.GetEntry("fabric.mod.json");
        await using var stream = zipEntry!.Open();
        using var raw = new StreamReader(stream);
        var content = await raw.ReadToEndAsync();

        var re = new FabricModInfo
        {
            Id = content.Fetch("id"),
            Name = content.Fetch("name"),
            Description = content.Fetch("description"),
            Version = content.Fetch("version"),
            Authors = (content.FetchJToken("authorList") ?? content.FetchJToken("authors"))?.Select(t => t.ToString()).ToList(),
            License = content.Fetch("license"),
            HomePage = content.Fetch("contact.homepage"),
            Issues = content.Fetch("contact.issues"),
            Sources = content.Fetch("contact.sources")
        };

        return re;
    }
}