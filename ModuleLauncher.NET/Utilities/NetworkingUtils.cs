using ModuleLauncher.NET.Models.Resources;

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
}