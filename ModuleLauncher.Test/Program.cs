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
using ModuleLauncher.Re.Models.Downloaders.Minecraft;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // var foobar = new AssetsLocator(@"C:\Users\ahpx\Desktop\MCD\.minecraft");
            //
            // var foo = new MinecraftDownloader(@"C:\Users\ahpx\Desktop\MCD\.minecraft");
            //
            // foo.DownloadStarted += e =>
            // {
            //     Console.WriteLine($"{e.FileName} started to download!");
            // };
            //
            // foo.DownloadCompleted += e =>
            // {
            //     Console.WriteLine("Download accomplished!");
            // };
            //
            // foo.DownloadProgressChanged += e =>
            // {
            //     Console.WriteLine(
            //         $"Progress: {e.ProgressPercentage:F1} {e.ReceivedBytesSize / 1024 :F1}KB/{e.TotalBytesToReceive / 1024 :F1}KB");
            // };
            //
            // await foo.DownloadMinecraft("1.16.5");
            //
            // var barfoo = await foobar.GetDependencies("1.16.5");
            // var bar = new DependenciesDownloader(barfoo);
            //
            // bar.DownloadCompleted += foo.DownloadCompleted;
            // bar.DownloadStarted += foo.DownloadStarted;
            // bar.DownloadProgressChanged += foo.DownloadProgressChanged;
            //
            // await bar.Download();
            //
            // foreach (var dependency in barfoo)
            // {
            //     Console.WriteLine(dependency.File.Exists);
            // }

            var foo = new Launcher(@"C:\Users\ahpx\Desktop\MCD\.minecraft")
            {
                Authentication = "AHpx",
                Java = @"C:\Program Files\Java\jre1.8.0_291\bin\javaw.exe"
            };

            var process = await foo.Launch("1.16.5");

            while (await process.StandardOutput.ReadLineAsync() != null)
            {
                Console.WriteLine(await process.StandardOutput.ReadLineAsync());
            }
        }
    }
}