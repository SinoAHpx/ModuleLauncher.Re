using Manganese.Data;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;

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
    public static MinecraftEntry GetInheritSource(this MinecraftEntry minecraftEntry)
    {
        return MinecraftResolver.Of(minecraftEntry).GetMinecraft(minecraftEntry.Json.InheritsFrom!)
            .ThrowIfNull(new CorruptedStuctureException($"Missing inherit source: {minecraftEntry.Json.InheritsFrom}"));
    }

    /// <summary>
    /// Throw a CorruptedStuctureException if object is null
    /// </summary>
    /// <param name="t">Could be MinecraftJson MinecraftEntry...</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ThrowCorruptedIfNull<T>(this T? t)
    {
        return t.ThrowIfNull(new CorruptedStuctureException("Minecraft json file corrupted"));
    }

    /// <summary>
    /// Check if minecraft entry has inheritFrom source 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <returns></returns>
    public static bool HasInheritSource(this MinecraftEntry minecraftEntry)
    {
        return !minecraftEntry.HasInheritSource();
    }
}