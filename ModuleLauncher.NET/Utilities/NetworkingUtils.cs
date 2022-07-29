using Flurl.Http;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Utilities;

public static class NetworkingUtils
{
    /// <summary>
    /// Convert string to DownloadSource member
    /// </summary>
    /// <param name="downloadSourceStr"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static DownloadSource ResolveDownloadSource(this string downloadSourceStr)
    {
        downloadSourceStr = downloadSourceStr.ToLower();

        return downloadSourceStr switch
        {
            "default" => DownloadSource.Default,
            "bmcl" => DownloadSource.Bmcl,
            "mcbbs" => DownloadSource.Mcbbs,
            _ => throw new ArgumentOutOfRangeException(nameof(downloadSourceStr), downloadSourceStr,
                "Source name currently can only be Default, Bmcl and Mcbbs")
        };
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

            var jsonFile = destinationDir.DiveToFile($"{remoteMinecraftEntry.Id}.json");
            await jsonFile.WriteAllTextAsync(json);

            return await remoteMinecraftEntry.ResolveLocalEntryAsync(resolver);
        }
    }
}