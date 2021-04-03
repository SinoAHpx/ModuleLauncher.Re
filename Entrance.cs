using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Authenticators;
using AHpx.ModuleLauncher.Data.Downloaders;
using AHpx.ModuleLauncher.Downloaders;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using AHpx.ModuleLauncher.Utils.Network;
using Downloader;
using MoreLinq;

namespace AHpx.ModuleLauncher
{
    public static class Entrance
    {
        public static async Task Main(string[] args)
        {
            
        }

        private static void Output<T>(this IEnumerable<T> ex)
        {
            Console.Write("[");
            foreach (var x1 in ex)
            {
                Console.Write($"{x1}, ");
            }

            Console.WriteLine("]");
        }
    }
}