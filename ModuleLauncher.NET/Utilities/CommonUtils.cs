using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ModuleLauncher.NET.Utilities;

public static class CommonUtils
{
    /// <summary>
    /// <example>Windows -> windows, Linux -> linux, MacOS -> osx</example>
    /// </summary>
    public static readonly string CurrentSystemName = GetCurrentSystem();

    public static readonly string SystemArch = Environment.Is64BitOperatingSystem ? "64" : "32";

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

    /// <summary>
    /// Dive into child directories
    /// </summary>
    /// <param name="parentDir"></param>
    /// <param name="childDirHierarchy">Child directories hierarchy, suppose to be split by "/"</param>
    /// <returns></returns>
    public static DirectoryInfo Dive(this DirectoryInfo parentDir, string childDirHierarchy)
    {
        var divePath = parentDir.FullName;
        if (childDirHierarchy.Contains('/'))
        {
            var split = childDirHierarchy.Split('/');
            divePath = split.Aggregate(divePath, (current, s) => current.AppendPath(s));
        }
        else
        {
            divePath = divePath.AppendPath(childDirHierarchy);
        }

        return new DirectoryInfo(divePath);
    }
    
    /// <summary>
    /// Dive into child directories
    /// </summary>
    /// <param name="parentDir"></param>
    /// <param name="childFileNameHierarchy">Hierarchy of sub directory, suppose to be split by "/"</param>
    /// <returns></returns>
    public static FileInfo DiveToFile(this DirectoryInfo parentDir, string childFileNameHierarchy)
    {
        var divePath = parentDir.FullName;
        if (childFileNameHierarchy.Contains('/'))
        {
            var split = childFileNameHierarchy.Split('/');
            divePath = split.Aggregate(divePath, (current, s) => current.AppendPath(s));
        }
        else
        {
            divePath = divePath.AppendPath(childFileNameHierarchy);
        }

        return new FileInfo(divePath);
    }

    /// <summary>
    /// Encapsulation of Path.Combine
    /// </summary>
    /// <param name="path"></param>
    /// <param name="toAppend"></param>
    /// <returns></returns>
    public static string AppendPath(this string path, string toAppend)
    {
        return Path.Combine(path, toAppend);
    }
}

