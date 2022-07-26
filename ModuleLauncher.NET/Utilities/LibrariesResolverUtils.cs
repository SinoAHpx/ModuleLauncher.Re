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
    /// <returns></returns>
    internal static string ResolveRawName(this string rawName, char separator = '/')
    {
        var segments = rawName.Split(":");
        if (segments.Length != 3)
        {
            throw new ErrorParsingLibraryException($"The raw name \"{rawName}\" of the lib is invalid");
        }
        
        var libOwner = segments[0].Replace('.', separator);
        var libName = segments[1];
        var libVersion = segments[2];

        var rawUrl = new[] { libOwner, libName, libVersion, $"{libName}-{libVersion}.jar" };
        var url = rawUrl.JoinToString(separator.ToString());

        return url;
    }
}