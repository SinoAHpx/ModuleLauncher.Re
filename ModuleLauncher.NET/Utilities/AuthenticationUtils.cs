using Manganese.Text;

namespace ModuleLauncher.NET.Utilities;

public static class AuthenticationUtils
{
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