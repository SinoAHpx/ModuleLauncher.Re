using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Authenticators;
using AHpx.ModuleLauncher.Data.Downloaders;
using AHpx.ModuleLauncher.Downloaders;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using AHpx.ModuleLauncher.Utils.Network;
using Downloader;
using MoreLinq;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher
{
    public static class Entrance
    {
        public static async Task Main(string[] args)
        {
            var locator = new MinecraftLocator
            {
                Location = @"C:\Users\ahpx\Desktop\Test\.minecraft",
                VersionIsolation = false
            };
            var mcd = new MinecraftDownloader
            {
                Locator = locator,
                DownloadSource = MinecraftDownloadSource.Official
            };
            mcd.StartedAction += startedArgs => Console.WriteLine($"{startedArgs.FileName} started!");
            mcd.ProgressAction += progressArgs =>
                Console.WriteLine($"{progressArgs.ReceivedBytesSize / 1000}/{progressArgs.TotalBytesSize / 1000}kb");
            mcd.CompletedAction += completedArgs => Console.WriteLine($"Download completed!");
            
            var launcher = new Launcher.Launcher
            {
                Locator = locator,
                Auth = "AHpx",
                JavaPath = @"C:\Program Files\Java\jre1.8.0_281\bin\javaw.exe"
            };
            var ver = "21w13a";

            await mcd.Download(ver);
            await mcd.DownloadAssets(ver, 64);
            await mcd.DownloadLibraries(ver, 16);
            
            var process = launcher.Launch(ver);
            
            while (!process.StandardOutput.ReadLine().IsNullOrEmpty())
            {
                Console.WriteLine(process.StandardOutput.ReadLine());
            }
        }

        private static void Output<T>(this IEnumerable<T> ex)
        {
            Console.Write("[");
            foreach (var x1 in ex)
            {
                Console.Write($"{x1}, ");
            }

            Console.WriteLine("]");
        }
    }
}