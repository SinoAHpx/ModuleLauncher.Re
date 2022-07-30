using System.IO.Compression;
using Flurl.Http;
using Manganese.Data;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Utilities;

public static class MinecraftUtils
{
    /// <summary>
    /// Get minecraft json type, mainly check by id property, if id contains specified keyword
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static MinecraftType GetMinecraftType(this MinecraftEntry minecraftEntry)
    {
        switch (minecraftEntry.Json.Type)
        {
            case MinecraftJsonType.Snapshot:
            case MinecraftJsonType.OldAlpha:
            case MinecraftJsonType.OldBeta:
                return MinecraftType.Vanilla;
            case MinecraftJsonType.Release:
                var id = minecraftEntry.Json.Id
                    .ThrowCorruptedIfNull()
                    .ToLower();
                if (id.Contains("forge"))
                    return MinecraftType.Forge;
                if (id.Contains("optifine"))
                    return MinecraftType.OptiFine;
                if (id.Contains("liteloader"))
                    return MinecraftType.LiteLoader;
                if (id.Contains("fabric"))
                    return MinecraftType.Fabric;

                return MinecraftType.Vanilla;
            default:
                return MinecraftType.Vanilla;
        }
    }

    /// <summary>
    /// Get inheritFrom minecraft
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static MinecraftEntry? GetInheritSource(this MinecraftEntry minecraftEntry)
    {
        if (minecraftEntry.HasInheritSource())
        {
            return MinecraftResolver.Of(minecraftEntry).GetMinecraft(minecraftEntry.Json.InheritsFrom!);
        }

        return null;
    }

    /// <summary>
    /// Throw a CorruptedStuctureException if object is null
    /// </summary>
    /// <param name="t">Could be MinecraftJson MinecraftEntry...</param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ThrowCorruptedIfNull<T>(this T? t, string? message = null)
    {
        message ??= "Minecraft json file corrupted";
        return t.ThrowIfNull(new CorruptedStuctureException(message));
    }

    /// <summary>
    /// Check if minecraft entry has inheritFrom source 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static bool HasInheritSource(this MinecraftEntry minecraftEntry)
    {
        return !minecraftEntry.Json.InheritsFrom.IsNullOrEmpty();
    }
    
    /// <summary>
    /// Get single remote Minecraft entry by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<RemoteMinecraftEntry> GetRemoteMinecraftAsync(string id)
    {
        try
        {
            var list = await GetRemoteMinecraftsAsync();

            return list.First(x => x.Id == id);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(
                $"Failed to request Minecraft {id}, it doesn't seem to be a valid Minecraft id", e);
        }
        
    }

    private static List<RemoteMinecraftEntry>? _remoteMinecraftEntriesCache;

    /// <summary>
    /// Get a list of remote Minecrafts
    /// <remarks>This method uses a internal cache variable to improve performance, if you need to refresh this variable, invoke <see cref="RefreshRemoteMinecraftsCacheAsync"/> </remarks>
    /// </summary>
    /// <returns></returns>
    public static async Task<List<RemoteMinecraftEntry>> GetRemoteMinecraftsAsync()
    {
        if (_remoteMinecraftEntriesCache != null)
            return _remoteMinecraftEntriesCache;

        var manifestUrl = "http://launchermeta.mojang.com/mc/game/version_manifest_v2.json";
        var manifest = await manifestUrl.GetStringAsync();
        var versions = manifest.Fetch("versions")!;
        var remoteEntries = JsonConvert.DeserializeObject<List<RemoteMinecraftEntry>>(versions)!;

        return remoteEntries;
    }

    /// <summary>
    /// Grab minecraft from remote and convert it to local
    /// </summary>
    /// <param name="resolver"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<MinecraftEntry> GetRemoteMinecraftAndToLocalAsync(this MinecraftResolver resolver, string id)
    {
        var remote = await GetRemoteMinecraftAsync(id);
        return await remote.ResolveLocalEntryAsync(resolver);
    }


    /// <summary>
    /// Refresh the internal cache variable
    /// </summary>
    /// <returns></returns>
    public static async Task<List<RemoteMinecraftEntry>> RefreshRemoteMinecraftsCacheAsync()
    {
        _remoteMinecraftEntriesCache = null;
        return await GetRemoteMinecraftsAsync();
    }

    /// <summary>
    /// Convert remote minecraft entry to local MinecraftEntry
    /// <remarks>Essentially this method fetch remote Minecraft json and write it to local</remarks>
    /// </summary>
    /// <param name="remoteMinecraftEntry"></param>
    /// <param name="resolver"></param>
    /// <returns></returns>
    public static async Task<MinecraftEntry> ResolveLocalEntryAsync(this RemoteMinecraftEntry remoteMinecraftEntry,
        MinecraftResolver resolver)
    {
        try
        {
            return resolver.GetMinecraft(remoteMinecraftEntry.Id);
        }
        catch
        {
            var json = await remoteMinecraftEntry.Url.GetStringAsync();
            var destinationDir =
                new DirectoryInfo(Path.Combine(resolver.RootPath, "versions", remoteMinecraftEntry.Id));
            destinationDir.Create();
            
            var jsonFile = destinationDir.DiveToFile($"{remoteMinecraftEntry.Id}.json");
            await jsonFile.WriteAllTextAsync(json);

            return await remoteMinecraftEntry.ResolveLocalEntryAsync(resolver);
        }
    }
    
    public static async Task ExtractNativesAsync(this MinecraftEntry minecraftEntry)
    {
        await Task.Run(() =>
        {
            var natives = minecraftEntry
                .GetLibraries().Where(l => l.IsNative)
                .ToList();

            if (!natives.Any())
                return;

            foreach (var native in natives)
            {
                var zipEntries = ZipFile.OpenRead(native.File.FullName).Entries;
                foreach (var zipArchiveEntry in zipEntries)
                {
                    if (Path.HasExtension(zipArchiveEntry.FullName))
                    {
                        var toExtract = minecraftEntry.Tree.Natives.DiveToFile(zipArchiveEntry.FullName);
                        toExtract.Directory?.Create();
                        if (!toExtract.Exists)
                        {
                            zipArchiveEntry.ExtractToFile(toExtract.FullName, true);
                        }
                    }
                }
            }
        });
    }
}