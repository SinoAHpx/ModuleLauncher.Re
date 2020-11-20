using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Downloaders;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using AHpx.ModuleLauncher.Utils.Network;

namespace AHpx.ModuleLauncher
{
    public static class Entrance
    {
        public static async Task Main(string[] args)
        {
            // var lo = new LibrariesLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
            // var lc = new Launcher
            // {
            //     Locator = lo,
            //     Auth = "AHpx",
            //     JavaPath = @"C:\Program Files\Java\jdk1.8.0_271\bin\javaw.exe",
            // };
            //
            // while (true)
            // {
            //     Console.WriteLine("Input version:");
            //     var ver = Console.ReadLine();
            //     Console.WriteLine(lc.GetArgument(ver));
            //
            //     var p = lc.Launch(ver);
            //     while (p.StandardOutput.ReadLine() != null)
            //     {
            //         Console.WriteLine(p.StandardOutput.ReadLine());
            //     }
            // }

            var core = new DownloaderCore();
            core.OnProgressChanged = (o, eventArgs) =>
            {
                Console.WriteLine(eventArgs.ProgressPercentage);
            };
            core.OnCompleted = (o, eventArgs) =>
            {
                Console.WriteLine("Download Complete!");
            };
            core.OnChunkProgressChanged = (o, eventArgs) =>
            {

            };

            await core.Download("https://download.mcbbs.net/mc/game/1.7.10/server/952438ac4e01b4d115c5fc38f891710c4941df29/server.jar", 
                new FileInfo($@"C:\Users\ahpx\Desktop\TEST.jar"));
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