using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Downloader;
using ModuleLauncher.Re.Downloaders;
using ModuleLauncher.Re.Launcher;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Downloaders;
using ModuleLauncher.Re.Models.Downloaders.Minecraft;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var librariesLocator = new LibrariesLocator(@"C:\Users\ahpx\Desktop\MCD\.minecraft");
            var assetsLocator = new AssetsLocator(@"C:\Users\ahpx\Desktop\MCD\.minecraft");

            var bar = await librariesLocator.GetDependencies("1.16.5-forge-36.1.32");

            bar = bar.Union(await assetsLocator.GetDependencies("1.16.5-forge-36.1.32"));

            var foo = new DependenciesDownloader(bar)
            {
                Source = DownloaderSource.Bmclapi
            };
            
            foo.DownloadStarted += e =>
            {
                Console.WriteLine($"{e.FileName} started to download!");
            };
            
            foo.DownloadCompleted += e =>
            {
                Console.WriteLine("Download accomplished!");
            };
            
            foo.DownloadProgressChanged += e =>
            {
                Console.WriteLine(
                    $"Progress: {e.ProgressPercentage:F1} {e.ReceivedBytesSize / 1024 :F1}KB/{e.TotalBytesToReceive / 1024 :F1}KB");
            };

            await foo.Download(20);

            var launcher = new Launcher(@"C:\Users\ahpx\Desktop\MCD\.minecraft")
            {
                Authentication = "Ahpx",
                Java = @"C:\Program Files\Java\jre1.8.0_291\bin\javaw.exe"
            };

            var process = await launcher.Launch("1.16.5-forge-36.1.32");

            while (await process.StandardOutput.ReadLineAsync() != null)
            {
                Console.WriteLine(await process.StandardOutput.ReadLineAsync());
            }
        }
    }
}