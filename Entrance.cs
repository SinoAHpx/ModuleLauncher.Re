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
            var lo = new LibrariesLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
            foreach (var library in lo.GetLibraries("1.16.4"))
            {
                Console.WriteLine(library.Name);
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