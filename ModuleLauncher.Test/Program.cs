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
            var root = @"C:\Users\ahpx\Desktop\MCD\.minecraft";
            var version = "fabric-loader-0.11.6-1.16.5";
            //
            // var mcd = new MinecraftDownloader(root);
            //
            // mcd.DownloadStarted += DownloadStarted;
            // mcd.DownloadCompleted += DownloadCompleted;
            // mcd.DownloadProgressChanged += DownloadProgressChanged;
            //
            // await mcd.DownloadMinecraft(version);
            
            var librariesLocator = new LibrariesLocator(root);
            var assetsLocator = new AssetsLocator(root);

            var bar = await librariesLocator.GetDependencies(version);

            bar = bar.Union(await assetsLocator.GetDependencies(version));

            var foo = new DependenciesDownloader(bar);
            
            foo.DownloadStarted += DownloadStarted;
            foo.DownloadCompleted += DownloadCompleted;
            foo.DownloadProgressChanged += DownloadProgressChanged;

            await foo.Download(20);

            var launcher = new Launcher(root)
            {
                Authentication = "Ahpx",
                Java = @"C:\Program Files\Java\jre1.8.0_291\bin\javaw.exe"
            };

            var process = await launcher.Launch(version);

            while (await process.StandardOutput.ReadLineAsync() != null)
            {
                Console.WriteLine(await process.StandardOutput.ReadLineAsync());
            }
        }

        private static void DownloadStarted(DownloadStartedEventArgs e)
        {
            Console.WriteLine($"{e.FileName} started to download!");
        }

        private static void DownloadCompleted(AsyncCompletedEventArgs e)
        {
            Console.WriteLine("Download accomplished!");
        }


        private static void DownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(
                $"Progress: {e.ProgressPercentage:F1}% - {e.ReceivedBytesSize / 1024 :F1}KB/{e.TotalBytesToReceive / 1024 :F1}KB");
        }
    }
}