using System;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.Minecraft.Locator;

namespace ModuleLauncher.Re.Utils
{
    public static class MinecraftHelper
    {
        public static MinecraftFileType GetMinecraftType(string name, MinecraftLocator locator)
        {
            var e = locator.GetMinecraftJsonEntity(name).id.ToLower();
            var a = locator.GetMinecraftVersionRoot(name);
            var v = Version.Parse("1.13");
            var t = locator.GetMinecraftJsonType(name);

            if (a == "legacy")
            {
                if (e.Contains("forge")) return MinecraftFileType.ForgeOld;

                if (e.Contains("optifine")) return MinecraftFileType.OptifineOld;

                if (e.Contains("liteloader")) return MinecraftFileType.LiteLoaderOld;

                return MinecraftFileType.VanillaOld;
            }

            var v1 = Version.Parse(a);
            if (e.Contains("forge")) return v1 < v ? MinecraftFileType.Forge : MinecraftFileType.ForgeNew;

            if (e.Contains("optifine")) return v1 < v ? MinecraftFileType.Optifine : MinecraftFileType.OptifineNew;

            if (e.Contains("fabric")) return MinecraftFileType.Fabric;

            if (e.Contains("liteloader")) return MinecraftFileType.LiteLoader;

            return v1 < v
                ? MinecraftFileType.Vanilla
                : MinecraftFileType.VanillaNew;
        }
    }
}