using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Downloader;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Downloaders;
using ModuleLauncher.Re.Launcher;
using ModuleLauncher.Re.Locators;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var downloader = new DownloaderT();

            await downloader.Download();
        }
    }

    class DownloaderT : DownloaderBase
    {
        protected override List<(string, FileInfo)> Files { get; set; }

        public DownloaderT()
        {
            Files = new List<(string, FileInfo)>
            {
                ("https://launcher.mojang.com/v1/objects/1cf89c77ed5e72401b869f66410934804f3d6f52/client.jar", 
                    new FileInfo(@"C:\Users\ahpx\Desktop\MinecraftsLab\files\test.jar"))
            };
        }

        protected override void DownloaderOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine($"{e.ProgressPercentage:F1}%");
        }

        protected override void DownloaderOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Console.WriteLine("Download accomplished!");
        }

        protected override void DownloaderOnDownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            Console.WriteLine("Download started!");
        }
    }
}