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

namespace AHpx.ModuleLauncher
{
    public static class Entrance
    {
        public static async Task Main(string[] args)
        {
            //TODO: Add the custom exception for each method

            #region Downloader initialized

            // var downloader = new Downloaders.Downloader();
            //
            
            //
            // var items = new List<DownloadItem>();
            // for (int i = 0; i < 10; i++)
            // {
            //     items.Add(new DownloadItem
            //     {
            //         Address = "http://ipv4.download.thinkbroadband.com/5MB.zip",
            //         FileName = @"C:\Users\ahpx\Desktop\Test\TestA_" + i + ".jar"
            //     });
            // }
            //
            // await downloader.Download(items, 3);

            #endregion

            var lcd = new MinecraftLocator(@"C:\Users\ahpx\Desktop\Test\.minecraft");
            var mcd = new MinecraftDownloader
            {
                Locator = lcd,
                DownloadSource = MinecraftDownloadSource.Official
            };
            
            mcd.StartedAction += startedArgs =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Download {startedArgs.FileName} stared!");
            };
            mcd.CompletedAction += completedArgs =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Download completed!");
            };
            mcd.ProgressAction += progressArgs =>
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(
                    $"The current downloading progress is {progressArgs.ReceivedBytesSize}/{progressArgs.TotalBytesSize} bytes");
            };
            
            var la = new Launcher.Launcher
            {
                JavaPath = @"C:\Program Files\Java\jre1.8.0_281\bin\javaw.exe",
                Auth = "AHpx",
                Locator = lcd
            };

            var ver = "1.13.2";

            await mcd.Download(ver);
            await mcd.DownloadLibraries(ver);
            await mcd.DownloadAssets(ver);

            var p = la.Launch(ver);

            while (await p.StandardOutput.ReadLineAsync() != null)
            {
                Console.WriteLine(await p.StandardOutput.ReadLineAsync());
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