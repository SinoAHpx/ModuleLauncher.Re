using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Minecraft;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Locators.Dependencies
{
    public class AssetsLocator : IDependenciesLocator
    {
        private readonly MinecraftLocator _locator;

        public AssetsLocator(MinecraftLocator locator)
        {
            _locator = locator;
        }

        public async Task<IEnumerable<Dependency>> GetDependencies(string id)
        {
            var mc = await _locator.GetLocalMinecraft(id);

            return await GetDependencies(mc);
        }

        public async Task<IEnumerable<Dependency>> GetDependencies(Minecraft mc)
        {
            if (mc.Raw.AssetId.IsNullOrEmpty())
            {
                if (mc.Raw.InheritsFrom.IsNullOrEmpty())
                {
                    throw new Exception($"{mc.Raw.Id} has no any valid assets!");
                }

                mc = await _locator.GetLocalMinecraft(mc.Raw.InheritsFrom);
            }
            
            var assetIndex = mc.Locality.AssetsIndexes.GetSubFileInfo($"{mc.Raw.AssetId}.json");

            if (!assetIndex.Exists)
            {
                var response = await HttpUtility.Get(mc.Raw.AssetIndexUrl ??
                                                     throw new Exception($"{mc.Raw.Id} without any assets index!"));

                await assetIndex.WriteAllText(response.Content);
            }

            return null;
        }
    }
}