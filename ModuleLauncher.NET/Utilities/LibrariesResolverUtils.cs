using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;

namespace ModuleLauncher.NET.Utilities;

/// <summary>
/// Utility methods for <see cref="LibrariesResolver"/>
/// </summary>
public static class LibrariesResolverUtils
{
    /// <summary>
    /// Resolve name of a library, convert a raw name to relative uri 
    /// <example>com.mojang:patchy:1.3.9 -> com/mojang/patchy/1.3.9/patchy-1.3.9.jar</example>
    /// </summary>
    /// <param name="rawName">com.mojang:patchy:1.3.9</param>
    /// <param name="suffix">$"{libName}-{libVersion}{suffix}.jar"</param>
    /// <returns>com/mojang/patchy/1.3.9/patchy-1.3.9.jar</returns>
    /// <exception cref="ErrorParsingLibraryException"></exception>
    internal static (string Name, string RelativeUrl, string RelativePath) ResolveRawName(this string rawName, string? suffix = null)
    {
        var segments = rawName.Split(":");
        if (segments.Length == 4)
        {
            suffix = segments[3];
            goto skipLengthCheck;
        }
        
        if (segments.Length != 3)
            throw new ErrorParsingLibraryException($"The raw name \"{rawName}\" of the lib is invalid");
        
        skipLengthCheck:
        if (!suffix.IsNullOrEmpty())
            suffix = $"-{suffix}";

        var libOwner = segments[0].Replace('.', '/');
        var libName = segments[1];
        var libVersion = segments[2];

        var rawUrl = new[] { libOwner, libName, libVersion, $"{libName}-{libVersion}{suffix}.jar" };
        var url = rawUrl.JoinToString("/");
        var path = url.Replace('/', Path.DirectorySeparatorChar);
        var name = path.GetFileName();

        return (name, url, path);
    }
}