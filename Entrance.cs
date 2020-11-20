using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Downloaders;
using AHpx.ModuleLauncher.Downloaders;
using AHpx.ModuleLauncher.Downloaders.Locator;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using AHpx.ModuleLauncher.Utils.Network;

namespace AHpx.ModuleLauncher
{
    public static class Entrance
    {
        public static async Task Main(string[] args)
        {
            var dl = new MinecraftDownloader
            {
                Source = DownloadSource.Mcbbs,
                OnProgressChanged = (o, eventArgs) => { Console.WriteLine(eventArgs.ProgressPercentage); },
                OnCompleted = (o, eventArgs) => { Console.WriteLine("Complete!"); }
            };
            
            var lo = new LibrariesLocator(@"C:\Users\ahpx\Desktop\Test\.minecraft");
            var ao = new AssetsLocator(lo.Location);
            
            var lc = new Launcher.Launcher
            {
                Locator = lo,
                LauncherName = "AHpx",
                JavaPath = @"C:\Program Files\Java\jdk1.8.0_271\bin\javaw.exe",
                Auth = "Test"
            };
            
            await dl.Download("1.8.9", lo);
            await dl.Download("1.8.9", ao);
            
            var pro = lc.Launch("1.8.9");
            
            while (await pro.StandardOutput.ReadLineAsync() != null)
            {
                Console.WriteLine(await pro.StandardOutput.ReadLineAsync());
            }
        }

        private static void Output<T>(this IEnumerable<T> ex)
        {
            foreach (var x1 in ex)
            {
                Console.WriteLine(x1);
            }
        }
    }
}