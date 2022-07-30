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

        var artifactUrl = entry.Raw.Fetch("downloads.artifact.url");
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

        var rawClient = minecraftEntry.Json.Raw.Fetch("downloads.client").ThrowCorruptedIfNull();
        var sha1 = rawClient.Fetch("sha1").ThrowCorruptedIfNull();
        if (downloadSource != DownloadSource.Default)
        {
            var combine = $"{downloadSource.GetDownloadSourcePrefix()}/mc/game/{minecraftEntry.Json.Id}/client/{sha1}/client.jar";
            return combine;
        }
        
        var url = rawClient.Fetch("url").ThrowCorruptedIfNull();
        return url;
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
}