using System;
using ModuleLauncher.Re.Models.Locators.Dependencies;

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

        /// <summary>
        /// Get current operating system type
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">Linux Windows Macos</exception>
        public static DependencySystem GetSystemType()
        {
            var platform = Environment.OSVersion.Platform;

            return platform switch
            {
                PlatformID.MacOSX => DependencySystem.Mac,
                PlatformID.Unix => DependencySystem.Linux,
                PlatformID.Win32NT => DependencySystem.Windows,
                PlatformID.Win32S => DependencySystem.Windows,
                PlatformID.Win32Windows => DependencySystem.Windows,
                PlatformID.WinCE => DependencySystem.Windows,
                PlatformID.Xbox => throw new ArgumentOutOfRangeException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public static char GetSystemSeparator()
        {
            var system = GetSystemType();
            
            return system switch
            {
                DependencySystem.Windows => '\\',
                DependencySystem.Linux => '/',
                DependencySystem.Mac => '/',
                _ => throw new ArgumentOutOfRangeException(nameof(system), system, null)
            };
        }
    }
}