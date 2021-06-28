using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Downloader;
using ModuleLauncher.Re.Downloaders;
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
        static async Task Main(string[] args)
        {
            Console.WriteLine(SystemUtility.GetSystemType().GetDependencySystemString());
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