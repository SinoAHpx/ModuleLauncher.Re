using System.IO.Compression;
using Flurl.Http;
using Manganese.Array;
using Manganese.Data;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Utilities;

public static class MinecraftUtils
{
    /// <summary>
    /// Get minecraft json type, mainly check by id property, if id contains specified keyword
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static MinecraftType GetMinecraftType(this MinecraftEntry minecraftEntry)
    {
        switch (minecraftEntry.Json.Type)
        {
            case MinecraftJsonType.Snapshot:
            case MinecraftJsonType.OldAlpha:
            case MinecraftJsonType.OldBeta:
                return MinecraftType.Vanilla;
            case MinecraftJsonType.Release:
                var id = minecraftEntry.Json.Id
                    .ThrowCorruptedIfNull()
                    .ToLower();
                if (id.Contains("forge"))
                    return MinecraftType.Forge;
                if (id.Contains("optifine"))
                    return MinecraftType.OptiFine;
                if (id.Contains("liteloader"))
                    return MinecraftType.LiteLoader;
                if (id.Contains("fabric"))
                    return MinecraftType.Fabric;

                return MinecraftType.Vanilla;
            default:
                return MinecraftType.Vanilla;
        }
    }

    /// <summary>
    /// Get inheritFrom minecraft
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static MinecraftEntry? GetInheritSource(this MinecraftEntry minecraftEntry)
    {
        if (minecraftEntry.HasInheritSource())
        {
            return MinecraftResolver.Of(minecraftEntry).GetMinecraft(minecraftEntry.Json.InheritsFrom!);
        }

        return null;
    }

    /// <summary>
    /// Throw a CorruptedStuctureException if object is null
    /// </summary>
    /// <param name="t">Could be MinecraftJson MinecraftEntry...</param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ThrowCorruptedIfNull<T>(this T? t, string? message = null)
    {
        message ??= "Minecraft json file corrupted";
        return t.ThrowIfNull(new CorruptedStuctureException(message));
    }

    /// <summary>
    /// Check if minecraft entry has inheritFrom source 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static bool HasInheritSource(this MinecraftEntry minecraftEntry)
    {
        return !minecraftEntry.Json.InheritsFrom.IsNullOrEmpty();
    }
    
    
    /// <summary>
    /// Extract natives files
    /// </summary>
    /// <param name="minecraftEntry"></param>
    public static async Task ExtractNativesAsync(this MinecraftEntry minecraftEntry)
    {
        await Task.Run(() =>
        {
            var natives = minecraftEntry
                .GetLibraries().Where(l => l.IsNative)
                .ToList();
            
            if (!natives.Any())
                return;
            
            foreach (var native in natives)
            {
                var zipEntries = ZipFile.OpenRead(native.File.FullName).Entries;
                foreach (var zipArchiveEntry in zipEntries)
                {
                    if (Path.HasExtension(zipArchiveEntry.FullName))
                    {
                        var toExtract = minecraftEntry.Tree.Natives.DiveToFile(zipArchiveEntry.FullName);
                        toExtract.Directory?.Create();
                        if (!toExtract.Exists)
                        {
                            zipArchiveEntry.ExtractToFile(toExtract.FullName, true);
                        }
                    }
                }
            }
        });
    }
}