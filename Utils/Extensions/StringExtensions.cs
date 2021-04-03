using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AHpx.ModuleLauncher.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string GetFileName(this string src)
        {
            return Path.GetFileName(src);
        }

        public static string RemoveAlphabets(this string ex)
        {
            foreach (Match match in Regex.Matches(ex, "[A-z]"))
            {
                ex = ex.Replace(match.Value, string.Empty);
            }

            return ex;
        }

        public static string ToLibraryFile(this string s, bool addExtension = true)
        {
            var split = s.Split(':');
            var sub = split[0].Split('.');

            return $@"{string.Join('\\', sub)}\{split[1]}\{split[2]}\{split[1]}-{split[2]}" +
                   (addExtension ? ".jar" : string.Empty);
        }

        public static bool IsNullOrEmpty(this string ex)
        {
            return string.IsNullOrEmpty(ex);
        }
    }
}