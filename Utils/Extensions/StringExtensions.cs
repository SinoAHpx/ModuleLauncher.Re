using System.IO;

namespace AHpx.ModuleLauncher.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string GetFileName(this string src)
        {
            return Path.GetFileName(src);
        }

        public static bool IsJar(this string src)
        {
            return Path.GetFileNameWithoutExtension(src) == Path.GetDirectoryName(src).GetFileName() && Path.GetExtension(src) == ".jar";
        }
        
        public static bool IsJson(this string src)
        {
            return Path.GetFileNameWithoutExtension(src) == Path.GetDirectoryName(src).GetFileName() && Path.GetExtension(src) == ".json";
        }
    }
}