using System;

namespace ModuleLauncher.Re.Utils
{
    public static class SystemUtility
    {
        /// <summary>
        /// Determine if current system is x64
        /// An encapsulation of Environment.Is64BitOperatingSystem
        /// </summary>
        /// <returns>for 64-bit system, the return result will be the string "64", and the 32-bit system is "32" as same</returns>
        public static string GetSystemBit()
        {
            return Environment.Is64BitOperatingSystem ? "64" : "32";
        }
    }
}