using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Locators;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var foo = new MinecraftLocator(@"C:\Users\ahpx\Desktop\MinecraftsLab\.minecraft");
            
            var bar = new LibrariesLocator(foo);
            
            foreach (var dependency in await bar.GetNativeDependencies("1.8.9"))
            {
                Console.WriteLine(dependency.RelativeUrl);
            }
        }
    }
}