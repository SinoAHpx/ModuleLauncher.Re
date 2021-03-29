using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            // downloader.StartedAction += startedArgs =>
            // {
            //     Console.ForegroundColor = ConsoleColor.Red;
            //     Console.WriteLine($"Download {startedArgs.FileName} stared!");
            // };
            // downloader.CompletedAction += completedArgs =>
            // {
            //     Console.ForegroundColor = ConsoleColor.Green;
            //     Console.WriteLine("Download completed!");
            // };
            // downloader.ProgressAction += progressArgs =>
            // {
            //     Console.ForegroundColor = ConsoleColor.Blue;
            //     Console.WriteLine(
            //         $"The current downloading progress is {progressArgs.ReceivedBytesSize}/{progressArgs.TotalBytesSize} bytes");
            // };
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

            var mcd = new MinecraftDownloader
            {
                Locator = new MinecraftLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft")
            };
            
            var re = await mcd.GetMinecraft("1.16.5");
            
            Console.WriteLine("File " +  re.File);
            Console.WriteLine("Inherit " + re.Inherit);
            Console.WriteLine("id " + re.Json.Id);
            Console.WriteLine("type " + re.Type);
            Console.WriteLine("root version " + re.RootVersion);
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