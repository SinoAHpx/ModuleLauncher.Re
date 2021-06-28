using System;
using System.Linq;
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
        internal static T ToJsonEntity<T>(this string json)
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
        internal static string Fetch(this JToken jObject, string path)
        {
            return jObject.SelectToken(path)?.ToString();
        }

        /// <summary>
        /// Fetch a json value from a string string via using a json path
        /// </summary>
        /// <param name="json"></param>
        /// <param name="path">xxx or xxx.xxx.xxx</param>
        /// <returns>return a JToken</returns>
        internal static JToken Fetch(this string json, string path)
        {
            return json.ToJObject().SelectToken(path);
        }

        /// <summary>
        /// Equals to JObject.ContainsKey if this token is a JObject
        /// </summary>
        /// <param name="token"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static bool IsPathExist(this JToken token, string key)
        {
            if (token is JObject jObject)
            {
                return jObject.SelectToken(key) != null;
            }

            throw new JsonException("This token is not a valid JObject!");
        }

        /// <summary>
        /// Convert a string to a JObject if it is a json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static JObject ToJObject(this string json)
        {
            return JObject.Parse(json);
        }
        
        /// <summary>
        /// Convert a string to a JArray object if it is json array
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        internal static JArray ToJArray(this string json)
        {
            return JArray.Parse(json);
        }
        
        /// <summary>
        /// Only available for modern version of minecraft
        /// Convert the arguments property of modern version minecraft to old-like minecraftArguments
        /// </summary>
        /// <param name="token">mc.Raw.Arguments</param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        internal static string ToNormalArguments(this JToken token)
        {
            var game = token["game"] ?? throw new JsonException($"Invalid json: {token}");
            var array = game.ToObject<JArray>();

            var arguments = array!.Where(x => x.Type == JTokenType.String);
            var re = string.Join(" ", arguments.Select(x => x.ToString()));

            return re.Trim();
        }
    }
}