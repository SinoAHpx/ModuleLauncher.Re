using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Downloader;
using ModuleLauncher.Re.Downloaders;
using ModuleLauncher.Re.Locators.Concretes;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var foo = new MinecraftDownloader();

            var bar = await foo.GetLatestVersions();

            Console.WriteLine(bar.Item1);
            Console.WriteLine(bar.Item2);
        }
    }
}