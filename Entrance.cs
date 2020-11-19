using System;
using System.Collections.Generic;
using System.Linq;
using AHpx.ModuleLauncher.Locators;

namespace AHpx.ModuleLauncher
{
    public static class Entrance
    {
        public static void Main(string[] args)
        {
            var lo = new AssetsLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
            foreach (var a in lo.GetAssets("20w46a"))
            {
                Console.WriteLine(a.File);
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