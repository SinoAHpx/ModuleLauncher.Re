using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Utils.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Convert a object to json string use JsonConvert.SerializeObject
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ignoreNull">Include null value or not</param>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool ignoreNull = true)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = ignoreNull ?  NullValueHandling.Ignore :  NullValueHandling.Include
            });
        }

        /// <summary>
        /// Convert a json to json entity via use JsonConvert.DeserializeObject
        /// </summary>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ToJsonEntity<T>(this string json)
        {
            try
            {
                var re = JsonConvert.DeserializeObject<T>(json);
                
                return re;
            }
            catch (Exception e)
            {
                throw new ArgumentException("The incoming json:\n" + json +
                                            "\ncannot be resolved to the specified entity!", e);
            }
        }

        /// <summary>
        /// Fetch a json value from JToken via using a json path
        /// </summary>
        /// <param name="jObject"></param>
        /// <param name="path">xxx or xxx.xxx.xx</param>
        /// <returns></returns>
        public static string Fetch(this JToken jObject, string path)
        {
            return jObject.SelectToken(path)?.ToString();
        }

        /// <summary>
        /// Fetch a json value from a string string via using a json path
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path">xxx or xxx.xxx.xxx</param>
        /// <returns>return a JToken</returns>
        public static JToken Fetch(this string json, string path)
        {
            return json.ToJObject().SelectToken(path);
        }

        /// <summary>
        /// Convert a string to a JObject if it is a json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JObject ToJObject(this string json)
        {
            return JObject.Parse(json);
        }
        
        /// <summary>
        /// Convert a string to a JArray object if it is json array
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JArray ToJArray(this string json)
        {
            return JArray.Parse(json);
        }
    }
}