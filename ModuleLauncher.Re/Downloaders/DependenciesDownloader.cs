using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Downloaders;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Utils.Extensions;
using MoreLinq;

namespace ModuleLauncher.Re.Downloaders
{
    public class DependenciesDownloader : DownloaderBase
    {
        internal DownloaderSource Source { get; set; }

        /// <summary>
        /// Download dependency one-by-one
        /// </summary>
        internal async Task Download(IEnumerable<Dependency> dependencies, bool ignoreExist = false)
        {
            foreach (var dependency in dependencies)
            {
                var url = GetDownloadUrl(dependency);

                await base.Download((url, dependency.File), ignoreExist);
            }
        }

        internal async Task DownloadParallel(IEnumerable<Dependency> dependencies, bool ignoreExist = false, int maxParallel = 5)
        {
            var files = dependencies.Select(x => (GetDownloadUrl(x), x.File));

            await base.DownloadParallel(files,ignoreExist, maxParallel);
        }

        /// <summary>
        /// Get download link via dependency object
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private string GetDownloadUrl(Dependency dependency)
        {
            if (dependency.IsLibraryDependency())
            {
                var url = $"https://libraries.minecraft.net/{dependency.RelativeUrl}";

                if (dependency.Raw != null && !dependency.Raw.IsPathExist("natives"))
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
                    DownloaderSource.Official => url,
                    DownloaderSource.Bmclapi => $"https://bmclapi2.bangbang93.com/maven/{dependency.RelativeUrl}",
                    DownloaderSource.Mcbbs => $"https://download.mcbbs.net/maven/{dependency.RelativeUrl}",
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return Source switch
            {
                DownloaderSource.Official => $"http://resources.download.minecraft.net/{dependency.RelativeUrl}",
                DownloaderSource.Bmclapi => $"https://bmclapi2.bangbang93.com/assets/{dependency.RelativeUrl}",
                DownloaderSource.Mcbbs => $"https://download.mcbbs.net/{dependency.RelativeUrl}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}