using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Downloader;
using ModuleLauncher.Re.Downloaders;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Downloaders.Minecraft;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var foo = new MinecraftDownloader(@"C:\Users\ahpx\Desktop\MCD\.minecraft");

            foo.DownloadStarted += e =>
            {
                Console.WriteLine($"{e.FileName} started to download!");
            };

            foo.DownloadCompleted += e =>
            {
                Console.WriteLine("Download accomplished!");
            };

            foo.DownloadProgressChanged += e =>
            {
                Console.WriteLine(
                    $"Progress: {e.ProgressPercentage:F1} {e.ReceivedBytesSize / 1024 / 1024:F1}MB/{e.TotalBytesToReceive / 1024 / 1024:F1}MB");
            };

            await foo.DownloadMinecraft("1.16.5");
        }
    }
}