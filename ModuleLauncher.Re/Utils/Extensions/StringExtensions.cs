using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Utils.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// A string extension method that equals to string.IsNullOrEmpty()
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Convert a raw dependence name to a relative url
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="rawName">e.g. commons-codec:commons-codec:1.9 or bdf48ef6b5d0d23bbb02e17d04865216179f510a</param>
        /// <param name="separator"></param>
        /// <returns></returns>
        internal static string GetRelativeUrl(this IDependenciesLocator locator, string rawName, string separator = "/")
        {
            if (locator is LibrariesLocator librariesLocator)
            {
                //<package>:<class>:<version>
                var colons = rawName.Split(":");
                var p = string.Join(separator, colons[0].Split("."));
                var c = colons[1];
                var v = colons[2];
                
                var result = $"{p}{separator}{c}{separator}{v}{separator}{c}-{v}.jar";

                return result;
            }

            return null;
        }

        /// <summary>
        /// Equals to Path.GetFileName
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static string GetFileName(this string s)
        {
            return Path.GetFileName(s);
        }

        /// <summary>
        /// Convert a enum to string via Description attribute
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        internal static string GetDependencySystemString(this DependencySystem system)
        {
            //stackoverflow oriented programming
            //https://stackoverflow.com/questions/630803/associating-enums-with-strings-in-c-sharp
            var attributes = (DescriptionAttribute[])system
                .GetType()
                .GetField(system.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        /// <summary>
        /// Append natives suffix for GetNativeDependencies
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        internal static string AppendNative(this IDependenciesLocator locator,  JToken token)
        {
            var rawName = token.Fetch("name") ??
                          throw new JsonException($"{token} is a unknown minecraft json format!");
            var rawNatives =
                $"{rawName}-{token.Fetch($"natives.{SystemUtility.GetSystemType().GetDependencySystemString()}").Replace("${arch}", SystemUtility.GetSystemBit())}";
            
            var appendedName = GetRelativeUrl(locator, rawNatives).GetFileName(); 
            
            var relativeUrl = GetRelativeUrl(locator, rawName);

            return relativeUrl.Replace(relativeUrl.GetFileName(), appendedName);
        }

        /// <summary>
        /// Incoming demo: xx/xx/xx/xxx, automatically convert to right directory separator
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        internal static string BuildPath(this string origin)
        {
            return origin.Replace("/", SystemUtility.GetSystemSeparator().ToString());
        }
    }
}