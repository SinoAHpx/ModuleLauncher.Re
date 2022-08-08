using Flurl.Http;
using Manganese.Array;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Utilities;

public static class DownloaderUtils
{
    /// <summary>
    /// Get download url of a library entry
    /// <remarks>Since most people don't really use OptiFine as a version, OptiFine itself contains no any download url, in this case, null will be returned</remarks>
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="downloadSource"></param>
    /// <returns></returns>
    public static string? GetDownloadUrl(this LibraryEntry entry,
        DownloadSource downloadSource = DownloadSource.Default)
    {
        if (downloadSource != DownloadSource.Default)
            return $"{downloadSource.GetDownloadSourcePrefix()}/{entry.RelativeUrl}";

        var fetchPath = entry.IsNative
            ? $"downloads.classifiers.natives-{CommonUtils.CurrentSystemName}.url"
            : "downloads.artifact.url";
        var artifactUrl = entry.Raw.Fetch(fetchPath);
        if (!artifactUrl.IsNullOrEmpty())
            return artifactUrl;

        var explicitUrl = entry.Raw.Fetch("url");
        if (!explicitUrl.IsNullOrEmpty())
        {
            if (explicitUrl.EndsWith("/"))
                return $"{explicitUrl}{entry.RelativeUrl}";

            return explicitUrl;
        }

        return entry.Type == MinecraftType.OptiFine ? null : $"https://libraries.minecraft.net/{entry.RelativeUrl}";
    }

    /// <summary>
    /// Get download url of vanilla Minecraft
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="downloadSource"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetDownloadUrl(this MinecraftEntry minecraftEntry,
        DownloadSource downloadSource = DownloadSource.Default)
    {
        if (minecraftEntry.GetMinecraftType() != MinecraftType.Vanilla)
            throw new InvalidOperationException("Only vanilla type of Minecraft can get download url");

        var downloadMeta = minecraftEntry.GetMinecraftClientDownloadInfo();
        if (downloadSource != DownloadSource.Default)
        {
            var combine =
                $"{downloadSource.GetDownloadSourcePrefix()}/mc/game/{minecraftEntry.Json.Id}/client/{downloadMeta.Sha1}/client.jar";
            return combine;
        }

        return downloadMeta.Url;
    }

    /// <summary>
    /// Get metadata in "downloads" json entry
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    private static (string Sha1, string Url) GetMinecraftClientDownloadInfo(this MinecraftEntry minecraftEntry)
    {
        var rawClient = minecraftEntry.Json.Raw.Fetch("downloads.client").ThrowCorruptedIfNull();
        var sha1 = rawClient.Fetch("sha1").ThrowCorruptedIfNull();
        var url = rawClient.Fetch("url").ThrowCorruptedIfNull();

        return (sha1, url);
    }

    /// <summary>
    /// Get download url of certain asset entry
    /// </summary>
    /// <param name="assetEntry"></param>
    /// <param name="downloadSource"></param>
    /// <returns></returns>
    public static string GetDownloadUrl(this AssetEntry assetEntry,
        DownloadSource downloadSource = DownloadSource.Default)
    {
        if (downloadSource == DownloadSource.Default)
            return $"http://resources.download.minecraft.net/{assetEntry.RelativeUrl}";

        return $"https://{downloadSource.GetDownloadSourcePrefix()}/{assetEntry.RelativeUrl}";
    }

    /// <summary>
    /// Get download prefix
    /// <example>download.mcbbs.net and bmclapi2.bangbang93.com</example>
    /// </summary>
    /// <param name="downloadSource"></param>
    /// <returns></returns>
    private static string GetDownloadSourcePrefix(this DownloadSource downloadSource)
    {
        //it can't be default
        return $"https://{(downloadSource == DownloadSource.Bmcl ? "bmclapi2.bangbang93.com" : "download.mcbbs.net")}";
    }

    /// <summary>
    /// Validate sha1 hash of library entry
    /// <remarks>If sha1 is not provided, true will be returned. If file does not exist, false will be returned</remarks>
    /// <returns>If validation passed, return true. Otherwise, return false.</returns>
    /// </summary>
    public static bool ValidateChecksum(this LibraryEntry libraryEntry)
    {
        if (!libraryEntry.File.Exists)
            return false;

        var fetchPath = libraryEntry.IsNative
            ? $"downloads.classifiers.natives-{CommonUtils.CurrentSystemName}.sha1"
            : "downloads.artifact.sha1";
        var sha1 = libraryEntry.Raw.Fetch(fetchPath);
        if (sha1.IsNullOrEmpty())
            return true;

        var realSha1 = libraryEntry.File.GetSha1();

        return sha1 == realSha1;
    }

    /// <summary>
    /// Validate sha1 value of Minecraft jar file
    /// <remarks>If Minecraft entry is not vanilla version, true will be returned since there's no sha1 provided by loader versions. If Minecraft does not exist, also return false.</remarks>
    /// <returns>If validation passed, return true. Otherwise, return false.</returns>
    /// </summary>
    public static bool ValidateChecksum(this MinecraftEntry minecraftEntry)
    {
        if (minecraftEntry.GetMinecraftType() != MinecraftType.Vanilla)
            return true;
        if (!minecraftEntry.Tree.Jar.Exists)
            return false;

        var sha1 = minecraftEntry.GetMinecraftClientDownloadInfo().Sha1;
        var realSha1 = minecraftEntry.Tree.Jar.GetSha1();

        return sha1 == realSha1;
    }

    /// <summary>
    /// Validate sha1 value of asset file
    /// <remarks>If file does not exist, false will be returned</remarks>
    /// </summary>
    /// <returns>If validation passed, return true. Otherwise, return false.</returns>
    /// <param name="assetEntry"></param>
    /// <returns></returns>
    public static bool ValidateChecksum(this AssetEntry assetEntry)
    {
        if (!assetEntry.File.Exists)
            return false;

        var realSha1 = assetEntry.File.GetSha1();

        return realSha1 == assetEntry.Hash;
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

        _remoteMinecraftEntriesCache = remoteEntries;
        return remoteEntries;
    }

    /// <summary>
    /// Filter remote minecrafts by type
    /// <remarks>Support multiple types of Minecraft</remarks>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type">Multiple types: MinecraftJsonType.Release | MinecraftJsonType.Snapshot</param>
    /// <returns></returns>
    public static async Task<List<RemoteMinecraftEntry>> FilterAsync(this Task<List<RemoteMinecraftEntry>> source,
        MinecraftJsonType type = MinecraftJsonType.Release | MinecraftJsonType.Snapshot | MinecraftJsonType.OldAlpha |
                                 MinecraftJsonType.OldBeta)
    {
        var entries = await source;

        return entries.Filter(type);
    }

    /// <summary>
    /// Filter remote minecrafts by type
    /// <remarks>Support multiple types of Minecraft</remarks>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="type">Multiple types: MinecraftJsonType.Release | MinecraftJsonType.Snapshot</param>
    /// <returns></returns>
    public static List<RemoteMinecraftEntry> Filter(this List<RemoteMinecraftEntry> source,
        MinecraftJsonType type = MinecraftJsonType.Release | MinecraftJsonType.Snapshot | MinecraftJsonType.OldAlpha |
                                 MinecraftJsonType.OldBeta)
    {
        return source.RemoveIf(e => !type.HasFlag(MinecraftJsonType.Release) && e.Type == MinecraftJsonType.Release)
            .RemoveIf(e =>
                !type.HasFlag(MinecraftJsonType.OldAlpha | MinecraftJsonType.OldBeta) &&
                e.Type is MinecraftJsonType.OldAlpha or MinecraftJsonType.OldBeta)
            .RemoveIf(e => !type.HasFlag(MinecraftJsonType.Snapshot) && e.Type == MinecraftJsonType.Snapshot)
            .ToList();
    }

    /// <summary>
    /// Grab minecraft from remote and convert it to local
    /// </summary>
    /// <param name="resolver"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<MinecraftEntry> GetRemoteMinecraftAndToLocalAsync(this MinecraftResolver resolver,
        string id)
    {
        try
        {
            return resolver.GetMinecraft(id);
        }
        catch
        {
            var remote = await GetRemoteMinecraftAsync(id);
            return await remote.ResolveLocalEntryAsync(resolver);
        }
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

    /// <summary>
    /// Convert string ("Default", "Bmcl" and "Mcbbs") to DownloadSource enum member
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
}