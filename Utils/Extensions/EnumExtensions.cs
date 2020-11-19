using AHpx.ModuleLauncher.Data.Locators;

namespace AHpx.ModuleLauncher.Utils.Extensions
{
    public static class EnumExtensions
    {
        internal static bool IsLoader(this Minecraft.MinecraftJson.MinecraftType ex)
        {
            return ex == Minecraft.MinecraftJson.MinecraftType.NewLoader ||
                   ex == Minecraft.MinecraftJson.MinecraftType.DefaultLoader;
        }
    }
}