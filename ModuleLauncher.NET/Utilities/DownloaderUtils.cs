using Manganese.Text;
using ModuleLauncher.NET.Models.Resources;

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
    public static string? GetDownloadUrl(this LibraryEntry entry, DownloadSource downloadSource = DownloadSource.Default)
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
    public static string GetDownloadUrl(this MinecraftEntry minecraftEntry, DownloadSource downloadSource = DownloadSource.Default)
    {
        if (minecraftEntry.GetMinecraftType() != MinecraftType.Vanilla)
            throw new InvalidOperationException("Only vanilla type of Minecraft can get download url");

        var downloadMeta = minecraftEntry.GetMinecraftClientDownloadInfo();
        if (downloadSource != DownloadSource.Default)
        {
            var combine = $"{downloadSource.GetDownloadSourcePrefix()}/mc/game/{minecraftEntry.Json.Id}/client/{downloadMeta.Sha1}/client.jar";
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
    public static string GetDownloadUrl(this AssetEntry assetEntry, DownloadSource downloadSource = DownloadSource.Default)
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
}