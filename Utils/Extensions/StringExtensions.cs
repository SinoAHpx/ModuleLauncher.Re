using System;
using System.IO;
using AHpx.ModuleLauncher.Data.Locators;

namespace AHpx.ModuleLauncher.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string GetFileName(this string src)
        {
            return Path.GetFileName(src);
        }
        //
        // internal static bool IsJar(this string src)
        // {
        //     return Path.GetFileNameWithoutExtension(src) == Path.GetDirectoryName(src).GetFileName() && Path.GetExtension(src) == ".jar";
        // }
        //
        // internal static bool IsJson(this string src)
        // {
        //     return Path.GetFileNameWithoutExtension(src) == Path.GetDirectoryName(src).GetFileName() && Path.GetExtension(src) == ".json";
        // }
        //
        // internal static bool IsNew(this string ex)
        // {
        //     try
        //     {
        //         var ver = Version.Parse(ex);
        //
        //         return ver >= new Version("1.13");
        //     }
        //     catch (Exception e)
        //     {
        //         return true;
        //     }
        // }
    }
}