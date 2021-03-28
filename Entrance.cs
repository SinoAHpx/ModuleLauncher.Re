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
            var downloader = new Downloaders.Downloader();

            downloader.StartedAction += startedArgs =>
            {
                Console.WriteLine($"Download {startedArgs.FileName} stared!");
            };
            downloader.CompletedAction += completedArgs =>
            {
                Console.WriteLine("Download completed!");
            };
            downloader.ProgressAction += progressArgs =>
            {
                Console.WriteLine(
                    $"The current downloading progress is {progressArgs.ReceivedBytesSize}/{progressArgs.TotalBytesSize} bytes");
            };

            await downloader.Download(
                "https://launcher.mojang.com/v1/objects/07ad8b23e33d195fd897133c9c0e35d7fa1593eb/client.jar",
                @"C:\Users\ahpx\Desktop\Test\Test.jar");

            // var fileNames = new string[]
            // {
            //     "Test_File_1",
            //     "Test_File_2",
            //     "Test_File_3",
            //     "AHpx_File_1",
            //     "AHpx_File_2",
            //     "AHpx_File_3",
            //     "Shit_File_1",
            //     "Shit_File_2",
            //     "Shit_File_3",
            //     "Single_File_1",
            // };
            //
            // await downloadParallel(fileNames);
            //
            // Console.WriteLine("Download completed!");
            // //await Task.WhenAll(t1("awd"), t1("awd1"), t1("awd2"));
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