using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Extensions
{
    public static class JsonExtensions
    {
        public static string ConvertUrl2Native(this JToken s)
        {
            return s.ToString().Replace("https://libraries.minecraft.net/", "").Replace('/', '\\');
        }

        public static bool IncludeStr(this JToken s, string key)
        {
            return s.ToString().Contains(key);
        }
    }
}