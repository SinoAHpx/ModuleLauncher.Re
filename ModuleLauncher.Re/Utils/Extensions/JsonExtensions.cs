using Newtonsoft.Json;

namespace ModuleLauncher.Re.Utils.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJsonString(this object obj, bool ignoreNull = true)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = ignoreNull ?  NullValueHandling.Ignore :  NullValueHandling.Include
            });
        }
    }
}