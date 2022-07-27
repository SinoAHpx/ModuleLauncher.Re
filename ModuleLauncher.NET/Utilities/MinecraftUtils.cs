using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Resources;

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
                    .ThrowIfNullOrEmpty<CorruptedStuctureException>("Minecraft json file may corrupted")
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
}