using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModuleLauncher.Re.Models.Locators;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Locators
{
    /// <summary>
    /// Locate a local or remote pure minecraft version
    /// </summary>
    public class LocalityLocator
    {
        private string _locality = @$"./.minecraft".BuildPath();

        /// <summary>
        /// Specify .minecraft directory, default value is .\.minecraft
        /// </summary>
        public string Locality
        {
            get => _locality ?? $"./.minecraft".BuildPath();
            set
            {
                if (!value.IsNullOrEmpty())
                {
                    if (!value.ToDirectoryInfo().Exists)
                    {
                        throw new Exception($"There's no such a directory named {value}");
                    }

                    _locality = value;
                }
                else
                {
                    _locality = $"./.minecraft".BuildPath();
                }
            }
        }

        public LocalityLocator(string locality = null)
        {
            Locality = locality;
        }

        /// <summary>
        /// Get a collection that contain specify locality versions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LocalVersion> GetLocalVersions()
        {
            var re = new List<LocalVersion>();

            var locality = Locality.ToDirectoryInfo();
            var versionsDir = locality.ToSubDirectoryInfo("versions");
            
            if (!versionsDir.Exists)
            {
                throw new IOException($"Directory {versionsDir} does not exist!");
            }
            
            var versions = versionsDir.GetDirectories();

            if (versions.Length <= 0)
            {
                throw new Exception($"There's no any local versions contain in {versionsDir}");
            }
            
            //info should be .minecraft/versions/%ver%
            foreach (var info in versions)
            {
                //we don't care if it doesn't exist
                var version = new LocalVersion
                {
                    Root = locality,
                    Versions = info.Parent,
                    Saves = locality.ToSubDirectoryInfo("saves"),
                    Mods = locality.ToSubDirectoryInfo("mods"),
                    ResourcesPacks = locality.ToSubDirectoryInfo("resourcepacks"),
                    TexturePacks = locality.ToSubDirectoryInfo("texturepacks"),
                    Libraries = locality.ToSubDirectoryInfo("libraries"),
                    Assets = locality.ToSubDirectoryInfo($"assets/objects".BuildPath()),
                    AssetsIndexes = locality.ToSubDirectoryInfo($"assets/indexes".BuildPath()),
                    Jar = $"{info}/{info.Name}.jar".BuildPath().ToFileInfo(),
                    Json = $"{info}/{info.Name}.json".BuildPath().ToFileInfo(),
                    Version = info,
                    Natives = info.ToSubDirectoryInfo("natives")
                };

                re.Add(version);
            }

            return re;
        }

        /// <summary>
        /// Get single version
        /// </summary>
        /// <param name="name">directory name of specify minecraft local version</param>
        /// <param name="ignoreExisting">if you don't even care if it exist</param>
        /// <returns></returns>
        public LocalVersion GetLocalVersion(string name, bool ignoreExisting = false)
        {
            if (ignoreExisting)
            {
                var locality = Locality.ToDirectoryInfo();
                var info = locality.ToSubDirectoryInfo($"versions{SystemUtility.GetSystemSeparator()}{name}");
                
                var version = new LocalVersion
                {
                    Root = locality,
                    Versions = info.Parent,
                    Saves = locality.ToSubDirectoryInfo("saves"),
                    Mods = locality.ToSubDirectoryInfo("mods"),
                    ResourcesPacks = locality.ToSubDirectoryInfo("resourcepacks"),
                    TexturePacks = locality.ToSubDirectoryInfo("texturepacks"),
                    Libraries = locality.ToSubDirectoryInfo("libraries"),
                    Assets = locality.ToSubDirectoryInfo($"assets/objects".BuildPath()),
                    AssetsIndexes = locality.ToSubDirectoryInfo($"assets/indexes".BuildPath()),
                    Jar = $"{info}/{info.Name}.jar".BuildPath().ToFileInfo(),
                    Json = $"{info}/{info.Name}.json".BuildPath().ToFileInfo(),
                    Version = info,
                    Natives = info.ToSubDirectoryInfo("natives")
                };

                return version;
            }
            
            try
            {
                var versions = GetLocalVersions().Where(x => x.Version.Name == name);

                return versions.First();
            }
            catch (Exception e)
            {
                throw new ArgumentException($"No such a local version called {name}", e);
            }
        }

        public static implicit operator LocalityLocator(string s)
        {
            return new LocalityLocator(s);
        }
    }
}