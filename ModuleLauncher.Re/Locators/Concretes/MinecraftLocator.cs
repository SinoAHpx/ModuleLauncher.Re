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

        /// <summary>
        /// Get local minecraft entities
        /// </summary>
        /// <returns>
        /// If there are some values that are not included in the json file,
        /// the corresponding property value is null
        /// </returns>
        public IEnumerable<Minecraft> GetLocalMinecrafts()
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
                    Raw = entity
                };
                
                re.Add(mc);
            }

            return re;
        }
    }
}