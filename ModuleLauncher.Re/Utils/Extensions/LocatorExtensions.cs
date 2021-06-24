using System.Collections.Generic;
using System.Threading.Tasks;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Minecraft;

namespace ModuleLauncher.Re.Utils.Extensions
{
    static class LocatorExtensions
    {
        /// <summary>
        /// Extension GetLibraries method for MinecraftLocator via local minecraft id
        /// Equals to librariesLocator.GetDependencies(mc)
        /// </summary>
        /// <param name="minecraftLocator"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static async Task<IEnumerable<Dependency>> GetLibraries(this MinecraftLocator minecraftLocator, string id)
        {
            var librariesLocator = new LibrariesLocator(minecraftLocator);
            var mc = await minecraftLocator.GetLocalMinecraft(id);

            return await librariesLocator.GetDependencies(mc);
        }

        /// <summary>
        /// Extension GetNatives method for MinecraftLocator via local minecraft id
        /// </summary>
        /// <param name="minecraftLocator"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static async Task<IEnumerable<Dependency>> GetNatives(this MinecraftLocator minecraftLocator, string id)
        {
            var librariesLocator = new LibrariesLocator(minecraftLocator);
            var mc = await minecraftLocator.GetLocalMinecraft(id);

            return await librariesLocator.GetNativeDependencies(mc); 
        }

        /// <summary>
        /// Determine if a minecraft entity has an inheritFrom proerty
        /// </summary>
        /// <returns></returns>
        internal static bool IsInherit(this Minecraft minecraft)
        {
            return !minecraft.Raw.InheritsFrom.IsNullOrEmpty();
        }
    }
}