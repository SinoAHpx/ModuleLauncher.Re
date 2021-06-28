using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Minecraft;

namespace ModuleLauncher.Re.Utils.Extensions
{
    internal static class LocatorExtensions
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

        /// <summary>
        /// Get inheritance of incoming minecraft
        /// </summary>
        /// <param name="minecraft"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<Minecraft> GetInherit(this Minecraft minecraft)
        {
            if (minecraft.IsInherit())
            {
                var locator = new MinecraftLocator(minecraft.Locality.Root.FullName);
                var re = await locator.GetLocalMinecraft(minecraft.Raw.InheritsFrom);

                return re;
            }

            throw new Exception("Incoming minecraft doesn't inherit from any minecraft!");
        }

        internal static bool IsLibraryDependency(this Dependency dependency)
        {
            return dependency.RelativeUrl.EndsWith(".jar");
        }
    }
}