using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Downloader;
using ModuleLauncher.Re.Downloaders;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var downloader = new DownloaderT();

            downloader.DownloadStarted += eventArgs =>
            {
                Console.WriteLine($"Start to downloading {eventArgs.FileName}");
            };
            downloader.DownloadProgressChanged += eventArgs =>
            {
                Console.WriteLine($"{eventArgs.ProgressId} is current {eventArgs.ProgressPercentage:F1}");
            };
            downloader.DownloadCompleted += eventArgs =>
            {
                Console.WriteLine($"Completed!");
            };

            await downloader.DownloadParallel(2);
        }
    }

    class DownloaderT : DownloaderBase
    {
        protected sealed override List<(string, FileInfo)> Files { get; set; }

        public DownloaderT()
        {
            Files = new List<(string, FileInfo)>
            {
                ("https://launcher.mojang.com/v1/objects/1cf89c77ed5e72401b869f66410934804f3d6f52/client.jar", 
                    new FileInfo(@"C:\Users\ahpx\Desktop\MinecraftsLab\files\test1.jar")),
                ("https://launcher.mojang.com/v1/objects/1cf89c77ed5e72401b869f66410934804f3d6f52/client.jar", 
                    new FileInfo(@"C:\Users\ahpx\Desktop\MinecraftsLab\files\test2.jar")),
                ("https://launcher.mojang.com/v1/objects/1cf89c77ed5e72401b869f66410934804f3d6f52/client.jar", 
                    new FileInfo(@"C:\Users\ahpx\Desktop\MinecraftsLab\files\test3.jar")),
            };
        }
    }
}