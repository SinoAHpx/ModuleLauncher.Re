using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Locators;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var lct = new MinecraftLocator(@"C:\Users\ahpx\Desktop\MinecraftsLab\.minecraft");
            
            var version = lct.GetMinecrafts();
            
            foreach (var minecraft in version)
            {
                if (minecraft.Json.Arguments != null)
                {
                    Console.WriteLine("Version:" + minecraft.Json.Id);
                    Console.WriteLine(minecraft.Json.Arguments.Fetch("game").ToJArray().Where(x => x.Type == JTokenType.String).ToJsonString());
                    Console.WriteLine(minecraft.Json.Libraries.Count);
                    Console.WriteLine(minecraft.Json.Type);
                    Console.WriteLine(minecraft.Json.AssetId);
                    Console.WriteLine(minecraft.Json.InheritsFrom);
                    Console.WriteLine(minecraft.Json.MainClass);
                    Console.WriteLine(minecraft.Json.MinecraftArguments);
                    Console.WriteLine(minecraft.Json.AssetIndexUrl);
                    Console.WriteLine();
                }
            }
        }
    }
}