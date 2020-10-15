using System.Collections.Generic;
using System.Linq;
using Masuit.Tools;
using ModuleLauncher.Re.Utils;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Extensions
{
    //TODO 记得把public都换成internal
    public static class JsonExtensions
    {
        /// <summary>
        ///     把某个JToken的文本值中的"https://libraries.minecraft.net/"字符串替换掉，并且转换成路径形式
        ///     <param name="s"></param>
        ///     <returns></returns>
        public static string ConvertUrl2Native(this JToken s)
        {
            return s.Replace("https://libraries.minecraft.net/", "").Replace('/', '\\');
        }

        /// <summary>
        ///     替换JToken的文本值，相当于JT.ToString().Replace()
        /// </summary>
        /// <param name="s"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string Replace(this JToken s, string oldValue, string newValue = "")
        {
            return s.ToString().Replace(oldValue, newValue);
        }

        /// <summary>
        ///     判断某个JToken的文本值是否包含某个字符串，相当于JT.ToString().Contains()
        /// </summary>
        /// <param name="s"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IncludeStr(this JToken s, string key)
        {
            return s.ToString().Contains(key);
        }

        /// <summary>
        ///     判断某个JToken中是否包含某个名称的属性
        /// </summary>
        /// <param name="s"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool IsPropertyExist(this JToken s, string propertyName)
        {
            var ps = new List<string>();
            s.ForEach(x =>
            {
                var str = x.ToString();
                var item = str.Remove(str.IndexOf(':')).Trim('"');
                ps.Add(item);
            });

            return ps.Any(x => x == propertyName);
        }

        /// <summary>
        ///     判断某个JObject中是否包含某个名称的属性
        /// </summary>
        /// <param name="s"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool IsPropertyExist(this JObject s, string propertyName)
        {
            return s.TryGetValue(propertyName, out _);
        }

        /// <summary>
        ///     获取JToken的某个属性值
        /// </summary>
        /// <param name="s"></param>
        /// <param name="propertyName">要获取的属性名</param>
        /// <param name="withReplace">是否把${arch}这个字符串替换成系统位数</param>
        /// <param name="throwEx">是否抛出异常</param>
        /// <returns></returns>
        public static string GetValue(this JToken s, string propertyName, bool withReplace = true, bool throwEx = false)
        {
            var re = throwEx ? s[propertyName].ToString() : s[propertyName]?.ToString();
            return withReplace ? re?.Replace("${arch}", SystemHelper.GetOsBitStr()) : re;
        }

        /// <summary>
        ///     模糊地获取JToken的某个属性值
        /// </summary>
        /// <param name="s"></param>
        /// <param name="propertyName">要获取的属性名</param>
        /// <param name="withReplace">是否把${arch}这个字符串替换成系统位数</param>
        /// <param name="throwEx">是否抛出异常</param>
        /// <returns></returns>
        public static IEnumerable<string> GetValueBlurry(this JToken s, string propertyName, bool withReplace = true,
            bool throwEx = false)
        {
            var jStr = s.ToString().Replace("{", "").Replace("}", "");
            var jStrArr = jStr.Split(',')
                .Select(x => "{" + x + "}")
                .Where(s1 => s1.Contains(propertyName));

            foreach (var s1 in jStrArr)
            {
                var re = throwEx ? s[propertyName].ToString() : s[propertyName]?.ToString();
                yield return withReplace ? re : re?.Replace("${arch}", SystemHelper.GetOsBitStr());
            }
        }

        /// <summary>
        ///     把name属性转换成路径或者url的形式
        /// </summary>
        /// <param name="src"></param>
        /// <param name="isNative">如果是native的话不会加拓展名(.jar)</param>
        /// <param name="isUrl">是不是Url</param>
        /// <returns></returns>
        public static string ToLibFormat(this JToken src, bool isNative = false, bool isUrl = false)
        {
            return src.ToString().ToLibFormat(isNative, isUrl);
        }

        /// <summary>
        ///     把已经转换的name属性转回去
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string[] ToSrcFormat(this JToken src)
        {
            return src.ToString().ToSrcFormat();
        }
    }
}