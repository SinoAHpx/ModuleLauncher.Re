using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static T ToJsonEntity<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string Fetch(this JObject jObject, string key)
        {
            return jObject[key].ToString();
        }
        
        public static string Fetch(this JToken jObject, string key)
        {
            return jObject[key].ToString();
        }
    }
}