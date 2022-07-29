using Manganese.Text;

namespace ModuleLauncher.NET.Utilities;

public static class AuthenticationUtils
{
    /// <summary>
    /// Extract code parameter from redirected url
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string ExtractCode(this string url)
    {
        try
        {
            return url.SubstringBetween("code=", "&lc=");
        }
        catch (Exception e)
        {
            return url.SubstringAfter("code=");
        }
    }
}