using System.IO;
using System.Runtime.CompilerServices;
using ModuleLauncher.Re.Locators.Dependencies;

namespace ModuleLauncher.Re.Utils.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// A string extension method that equals to string.IsNullOrEmpty()
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
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
        public static string GetRelativeUrl(this IDependenciesLocator locator, string rawName, string separator = "/")
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
        public static string GetFileName(this string s)
        {
            return Path.GetFileName(s);
        }
    }
}