using System;
using System.Linq;
using AHpx.ModuleLauncher.Locators;

namespace AHpx.ModuleLauncher
{
    public class Entrance
    {
        public static void Main(string[] args)
        {
            var lo = new LibrariesLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
            lo.GetLibraries("1.8").ToList().ForEach(Console.Write);
        }
    }
}