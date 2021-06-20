using System;
using System.IO;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Locators;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var lct = new VersionLocator(@"D:\Minecraft\Solution1\.minecraft");
            foreach (var version in lct.GetLocalVersions())
            {
                Console.WriteLine("=========================================================");
                Console.WriteLine(version.Root);
                Console.WriteLine(version.Versions);
                Console.WriteLine(version.Saves);
                Console.WriteLine(version.Mods);
                Console.WriteLine(version.ResourcesPacks);
                Console.WriteLine(version.TexturePacks);
                Console.WriteLine(version.Libraries);
                Console.WriteLine(version.Assets);
                Console.WriteLine(version.AssetsIndexes);
                Console.WriteLine(version.Jar);
                Console.WriteLine(version.Json);
                Console.WriteLine(version.VersionDir);
                Console.WriteLine(version.Natives);
                Console.WriteLine("=========================================================");
                Console.WriteLine();
            }
        }
    }
}