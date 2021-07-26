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
            var root = @"C:\Users\ahpx\AppData\Roaming\.minecraft";
            var ver = "1.17.1-forge-37.0.8";

            // var mcd = new MinecraftDownloader(root)
            // {
            //     
            // };
            // mcd.DownloadCompleted += DownloadCompleted;
            // mcd.DownloadStarted += DownloadStarted;
            // mcd.DownloadProgressChanged += DownloadProgressChanged;
            // mcd.OnRetry += OnRetry;
            // await mcd.Download(ver);
            
            var downloader = new AssetsDownloader(root)
            {
                
            };
            
            var libd = new LibrariesDownloader(root)
            {
                
            };
            
            downloader.DownloadStarted += DownloadStarted;
            downloader.DownloadCompleted += DownloadCompleted;
            downloader.DownloadProgressChanged += DownloadProgressChanged;
            downloader.OnRetry += OnRetry;
            
            libd.DownloadCompleted   += DownloadCompleted;
            libd.DownloadStarted    += DownloadStarted;
            libd.DownloadProgressChanged    += DownloadProgressChanged;
            libd.OnRetry += OnRetry;
            
            await downloader.DownloadParallel(ver, false, 32);
            await libd.DownloadParallel(ver, false, 8);

            var la = new Launcher(root)
            {
                Authentication = Guid.NewGuid().ToString("N"),
                Java = @"C:\Program Files (x86)\Minecraft Launcher\runtime\java-runtime-alpha\windows-x64\java-runtime-alpha\bin\javaw.exe",
                MaximumMemorySize = 4096
            };

            var process = await la.Launch(ver);
            
            process.Exited += (sender, eventArgs) =>
            {
                Console.WriteLine($"Process exited with code {process.ExitCode}");
            };

            while (!process.HasExited)
            {
                var output = await process.StandardOutput.ReadLineAsync();

                if (!string.IsNullOrEmpty(output))
                {
                    Console.WriteLine(await process.StandardOutput.ReadLineAsync());
                }
            }
            
            // process.WaitForExit();
        }

        private static void OnRetry(Exception arg1, int arg2)
        {
            Console.WriteLine($"Exception occurred: {arg1.Message}, this is the {arg2} times of retry");
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