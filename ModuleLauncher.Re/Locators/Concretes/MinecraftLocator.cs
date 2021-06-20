using System.Collections.Generic;
using ModuleLauncher.Re.Models.Locators.Minecraft;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Locators.Concretes
{
    public class MinecraftLocator : VersionLocator
    {
        public MinecraftLocator(string locality = null) : base(locality)
        {
        }

        public IEnumerable<Minecraft> GetMinecrafts()
        {
            var re = new List<Minecraft>();
            var versions = GetLocalVersions();
            
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