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
        if (downloadSource == DownloadSource.Bmcl)
            return $"https://bmclapi2.bangbang93.com/maven/{entry.RelativeUrl}";
        if (downloadSource == DownloadSource.Mcbbs)
            return $"https://download.mcbbs.net/maven/{entry.RelativeUrl}";

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

    public static string? GetDownloadUrl(this MinecraftEntry minecraftEntry, DownloadSource downloadSource = DownloadSource.Default)
    {
        if (minecraftEntry.GetMinecraftType() != MinecraftType.Vanilla)
            throw new InvalidOperationException("Only vanilla type of Minecraft can get download url");

        var rawClient = minecraftEntry.Json.Raw.Fetch("downloads.client").ThrowCorruptedIfNull();
        var sha1 = rawClient.Fetch("sha1").ThrowCorruptedIfNull();
        if (downloadSource != DownloadSource.Default)
        {
            var prefix = downloadSource == DownloadSource.Bmcl ? "bmclapi2.bangbang93.com" : "download.mcbbs.net";
            var combine = $"https://{prefix}/mc/game/{minecraftEntry.Json.Id}/client/{sha1}/client.jar";
            return combine;
        }
        
        var url = rawClient.Fetch("url").ThrowCorruptedIfNull();
        return url;
    }
}