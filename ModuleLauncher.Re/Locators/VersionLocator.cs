using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModuleLauncher.Re.Models.Locators;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Locators
{
    /// <summary>
    /// Locate a local or remote pure minecraft version
    /// </summary>
    public class VersionLocator
    {
        /// <summary>
        /// Specify .minecraft directory, default value is .\.minecraft
        /// </summary>
        public string Locality { get; set; } = @".\.minecraft";
        
        public VersionLocator(string locality = null)
        {
            Locality = locality;
        }

        public IEnumerable<Version> GetVersions()
        {
            var re = new List<Version>();
            var versions = Directory.GetDirectories($@"{Locality}\versions").Select(x => x.ToDirectoryInfo());
            
            //info should be .minecraft/versions/%ver%
            foreach (var info in versions)
            {
                var locality = Locality.ToDirectoryInfo(); ;

                var version = new Version
                {
                    Root = locality,
                    Versions = info.Parent,
                    Saves = locality.ToSubDirectoryInfo("saves"),
                    Mods = locality.ToSubDirectoryInfo("mods"),
                    ResourcesPacks = locality.ToSubDirectoryInfo("resourcepacks"),
                    TexturePacks = locality.ToSubDirectoryInfo("texturepacks"),
                    Libraries = locality.ToSubDirectoryInfo("libraries"),
                    Assets = locality.ToSubDirectoryInfo("assets\\objects"),
                    AssetsIndexes = locality.ToSubDirectoryInfo("assets\\indexes"),
                    Jar = $"{info}\\{info.Name}.jar".ToFileInfo(),
                    Json = $"{info}\\{info.Name}.json".ToFileInfo(),
                    VersionDir = info,
                    Natives = info.ToSubDirectoryInfo("natives")
                };

                re.Add(version);
            }

            return re;
        } 
    }
}