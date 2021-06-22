using System.Collections.Generic;
using ModuleLauncher.Re.Models.Locators.Minecraft;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Locators.Concretes
{
    public class MinecraftLocator
    {
        private readonly LocalityLocator _locator;

        public MinecraftLocator(LocalityLocator locator)
        {
            _locator = locator;
        }

        public IEnumerable<Minecraft> GetMinecrafts()
        {
            var re = new List<Minecraft>();
            var versions = _locator.GetLocalVersions();
            
            foreach (var version in versions)
            {
                var json = version.Json.ReadAllText();
                var entity = json.ToJsonEntity<MinecraftJson>();

                var mc = new Minecraft
                {
                    Locality = version,
                    Json = entity
                };
                
                re.Add(mc);
            }

            return re;
        }
    }
}