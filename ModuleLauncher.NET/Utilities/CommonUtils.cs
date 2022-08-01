using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Manganese.Text;

namespace ModuleLauncher.NET.Utilities;

public static class CommonUtils
{
    /// <summary>
    /// <example>Windows -> windows, Linux -> linux, MacOS -> osx</example>
    /// </summary>
    public static readonly string CurrentSystemName = GetCurrentSystem();

    /// <summary>
    /// "64" or "32"
    /// </summary>
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

    /// <summary>
    /// If first replace candidate is null, second one will be used, second candidate can be also null, if so, nothing will be replaced
    /// </summary>
    /// <param name="source"></param>
    /// <param name="placeholder"></param>
    /// <param name="toReplaceWith"></param>
    /// <param name="backupReplace"></param>
    /// <returns></returns>
    public static string ReplaceIfNull(this string source, string placeholder, string? toReplaceWith, string? backupReplace)
    {
        if (!toReplaceWith.IsNullOrEmpty())
            return source.Replace(placeholder, toReplaceWith);

        if (backupReplace.IsNullOrEmpty())
            return source;

        return source.Replace(placeholder, backupReplace);
    }

    /// <summary>
    /// Try to get java executable file path
    /// </summary>
    /// <param name="javaExeFile"></param>
    /// <returns></returns>
    public static int GetJavaExecutableVersion(this FileInfo javaExeFile)
    {
        if (!javaExeFile.Exists)
        {
            throw new NullReferenceException("Java executable file does not exist");
        }
        var versionInfo = FileVersionInfo.GetVersionInfo(javaExeFile.FullName);
        var version =
            versionInfo.FileVersion.ThrowIfNullOrEmpty<NullReferenceException>(
                $"Specified java exe {javaExeFile} might be corrupted");
        if (!version.Contains('.'))
        {
            return version.ToInt32();
        }

        var split = version.Split('.');

        return split.First().ToInt32();
    }

    /// <summary>
    /// Convert unix milliseconds timestamp to DateTime
    /// </summary>
    /// <param name="timestamp"></param>
    /// <returns></returns>
    public static DateTime? UnixTimeStampToDateTime(string? timestamp)
    {
        if (timestamp.IsNullOrEmpty())
        {
            return null;
        }
        
        var offset = DateTimeOffset.FromUnixTimeMilliseconds(
            timestamp.ThrowIfNotInt64<InvalidOperationException>($"{timestamp} is not a valid long integer"));

        return offset.DateTime;
    }

    public static string GetSha1(this FileInfo file, bool capitalized = false)
    {
        using var stream = file.OpenRead();
        var sha1 = SHA1.Create().ComputeHash(stream);
        
        var toReturn = BitConverter.ToString(sha1).Empty("-");
        if (!capitalized)
            toReturn = toReturn.ToLower();

        return toReturn;
    }
}

