using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Downloaders;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Downloaders
{
    public class DependenciesDownloader : DownloaderBase
    {
        public IEnumerable<Dependency> Dependencies { get; set; }

        public DownloaderSource Source { get; set; }
        
        protected override List<(string, FileInfo)> Files { get; set; } = new List<(string, FileInfo)>();

        public DependenciesDownloader(IEnumerable<Dependency> dependencies)
        {
            Dependencies = dependencies;
        }

        public async Task Download(int parallelCount = 5)
        {
            foreach (var dependency in Dependencies)
            {
                if (!dependency.File.Exists)
                {
                    Files.Add((GetDownloadUrl(dependency), dependency.File));
                }
            }

            await base.DownloadParallel(parallelCount); 
        }

        private string GetDownloadUrl(Dependency dependency)
        {
            if (dependency.IsLibraryDependency())
            {
                return Source switch
                {
                    DownloaderSource.Mojang => $"https://libraries.minecraft.net/{dependency.RelativeUrl}",
                    DownloaderSource.Bmclapi => $"https://bmclapi2.bangbang93.com/maven/{dependency.RelativeUrl}",
                    DownloaderSource.Mcbbs => $"https://download.mcbbs.net/maven/{dependency.RelativeUrl}",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return Source switch
            {
                DownloaderSource.Mojang => $"http://resources.download.minecraft.net/{dependency.RelativeUrl}",
                DownloaderSource.Bmclapi => $"https://bmclapi2.bangbang93.com/assets/{dependency.RelativeUrl}",
                DownloaderSource.Mcbbs => $"https://download.mcbbs.net/{dependency.RelativeUrl}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}