using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using Manganese.Array;
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
            return await GetModInfoLegacyForgeAsync(mod, archive);
        if (archive.Entries.Any(e => e.Name == "mods.toml"))
            return await GetModInfoForgeAsync(mod, archive);
        if (archive.Entries.Any(e => e.Name is "fabric.mod.json" or "quilt.mod.json"))
            return getModInfoFabricAsync(mod, archive);

        throw new UnknownModException("Unknown mod or mod is corrupted");
    }

    private static async Task<ModInfo> GetModInfoLegacyForgeAsync(FileInfo mod, ZipArchive archive)
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
    
    private static async Task<ModInfo> GetModInfoForgeAsync(FileInfo mod, ZipArchive archive)
    {
        var zipEntry = archive.GetEntry("META-INF/mods.toml")!;
        await using var stream = zipEntry.Open();
        using var raw = new StreamReader(stream);
        var tomTable = TOML.Parse(raw);
        var modNode = tomTable["mods"].AsArray[0];
        var re = new ForgeModInfo
        {
            Id = modNode["modId"],
            Version = modNode["version"],
            Name = modNode["displayName"],
            Url = modNode["displayURL"],
            License = tomTable["license"],
            Description = modNode["description"],
            Authors = modNode.IsArray
                ? modNode["authors"].AsArray.RawArray.Select(x => x.ToString()).ToList()
                : new List<string> { modNode["authors"] },
        };
        
        return re;
    }

    private static ModInfo getModInfoFabricAsync(FileInfo mod, ZipArchive archive)
    {
        return new ModInfo();
    }
}