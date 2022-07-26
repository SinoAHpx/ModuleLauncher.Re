using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ModuleLauncher.NET.Utilities;

public static class CommonUtils
{
    /// <summary>
    /// <example>Windows -> windows, Linux -> linux, MacOS -> osx</example>
    /// </summary>
    public static string CurrentSystemName => GetCurrentSystem();

    public static string? GetDescription(this Enum t)
    {
        var type = t.GetType();
        var field = type.GetField(t.ToString());
        var description = field?.GetCustomAttribute<DescriptionAttribute>();

        return description?.Description;
    }
    
    private static string GetCurrentSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return "windows";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)
            || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "linux";
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "osx";
        }

        throw new SystemException("Unsupported operating system");
    }
}