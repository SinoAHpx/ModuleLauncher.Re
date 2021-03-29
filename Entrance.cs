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
            // var input = new string[] {"AHpx1", "AHpx2", "Ahpx3", "Ahpx4", "Ahpx5"};
            // await downloadParallel(input);


            var downloader = new Downloaders.Downloader();
            
            downloader.StartedAction += startedArgs =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Download {startedArgs.FileName} stared!");
            };
            downloader.CompletedAction += completedArgs =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Download completed!");
            };
            downloader.ProgressAction += progressArgs =>
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(
                    $"The current downloading progress is {progressArgs.ReceivedBytesSize}/{progressArgs.TotalBytesSize} bytes");
            };
            
            var items = new List<DownloadItem>();
            for (int i = 0; i < 10; i++)
            {
                items.Add(new DownloadItem
                {
                    Address = "http://ipv4.download.thinkbroadband.com/5MB.zip",
                    FileName = @"C:\Users\ahpx\Desktop\Test\TestA_" + i + ".jar"
                });
            }
            
            await downloader.Download(items, 3);
        }

        private static async Task downloadParallel(string[] fileNames, int maxCount = 3)
        {
            var name = fileNames.Batch(maxCount);
            var tasks = new List<Task>();
            
            foreach (var enumerable in name)
            {
                foreach (var s in enumerable)
                {
                    tasks.Add(t1(s));
                }

                await Task.WhenAll(tasks);
                tasks.Clear();
            }
        }

        private static async Task t1(string name)
        {
            var loc = @"C:\Users\ahpx\Desktop\Test\";

            var download = new DownloadService(new DownloadConfiguration
            {
                ParallelDownload = true
            });

            download.DownloadStarted += (sender, eventArgs) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"This download {Path.GetFileName(eventArgs.FileName)} is started!!!");
            };
            download.DownloadFileCompleted += (sender, eventArgs) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"This download is finished!!!");
            };
            download.DownloadProgressChanged += (sender, eventArgs) =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(
                    $"The current downloading progress is {eventArgs.ReceivedBytesSize}/{eventArgs.TotalBytesToReceive} bytes");
            };

            await download.DownloadFileTaskAsync("http://ovh.net/files/1Mio.dat", $"{loc}{name}.txt");
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