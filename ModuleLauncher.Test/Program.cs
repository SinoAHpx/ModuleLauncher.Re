using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Downloader;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Downloaders;
using ModuleLauncher.Re.Downloaders.Concrete;
using ModuleLauncher.Re.Launcher;
using ModuleLauncher.Re.Locators;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Downloaders;
using ModuleLauncher.Re.Models.Downloaders.Minecraft;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Test
{
    class Program
    {
        //java executable file: 
        static async Task Main(string[] args)
        {
            var r = 0;

            var action = new Action<int>(Console.WriteLine);

            var tasks = new List<Task>();
            foreach (var i in Enumerable.Range(1, 10))
            {
                var downloader = new DownloadUtility
                {
                    DownloadInfo = ("https://bmclapi2.bangbang93.com/version/1.8.9/client",
                        new FileInfo(@$"C:\Users\ahpx\Desktop\MCD\file_{i}"))
                };

                downloader.DownloadCompleted += DownloadCompleted;

                tasks.Add(downloader.Download());
            }

            await Task.WhenAll(tasks);

            // var downloader = new AssetsDownloader(@"C:\Users\ahpx\Desktop\MCD\.minecraft")
            // {
            //     Source = DownloaderSource.Bmclapi
            // };
            //
            // // downloader.DownloadStarted += DownloadStarted;
            // downloader.DownloadCompleted += DownloadCompleted;
            // // downloader.DownloadProgressChanged += DownloadProgressChanged;
            // downloader.OnRetry += (exception, i) =>
            // {
            //     Console.WriteLine($"Exception occured: {exception.Message}, this is the {i} times of retry");
            // };
            //
            // await downloader.DownloadParallel("1.8.9", false, 8);
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