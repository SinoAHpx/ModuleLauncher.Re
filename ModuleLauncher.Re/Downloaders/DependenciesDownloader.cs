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
                var url = GetDownloadUrl(dependency);

                if (!dependency.File.Exists)
                {
                    Files.Add((url, dependency.File));
                }
            }

            await base.DownloadParallel(parallelCount); 
        }

        private string GetDownloadUrl(Dependency dependency)
        {
            if (dependency.IsLibraryDependency())
            {
                var url = $"https://libraries.minecraft.net/{dependency.RelativeUrl}";

                if (dependency.Raw != null)
                {
                    if (dependency.Raw.IsPathExist("url"))
                    {
                        var rootUrl = dependency.Raw.Fetch("url").TrimEnd('/');
                        
                        if (rootUrl.EndsWith(".jar"))
                        {
                            url = rootUrl;
                        }
                        else
                        {
                            url = $"{rootUrl}/{dependency.RelativeUrl}";
                        }
                        
                    }

                    if (dependency.Raw.IsPathExist("downloads.artifact"))
                    {
                        if (!dependency.Raw.Fetch("downloads.artifact.url").IsNullOrEmpty())
                        {
                            url = dependency.Raw.Fetch("downloads.artifact.url");
                        }
                        else
                        {
                            url = $"https://bmclapi2.bangbang93.com/maven/{dependency.RelativeUrl}";
                        }
                    }
                }
                
                return Source switch
                {
                    DownloaderSource.Mojang => url,
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