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
    }
}