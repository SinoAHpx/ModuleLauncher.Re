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
            var lc = new MinecraftLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
            
            lc.GetAssets("1.7.10-Forge10.13.4.1614-1.7.10").ForEach(x =>
            {
                Console.WriteLine(x.File);
            });
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