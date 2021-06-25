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
            var foo = new MinecraftDownloader();

            var bar = await foo.GetRemoteMinecrafts();
            
            foreach (var item in bar.Where(x => x.Type == MinecraftDownloadType.OldAlpha))
            {
                Console.WriteLine(item.ToJsonString());
            }
        }
    }
}