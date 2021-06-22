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
            
            var version = lct.GetLocalMinecrafts();
            
            foreach (var minecraft in version)
            {
                if (minecraft.Raw.Arguments != null)
                {
                    Console.WriteLine("Version:" + minecraft.Raw.Id);
                    Console.WriteLine(minecraft.Raw.Arguments.Fetch("game").ToJArray().Where(x => x.Type == JTokenType.String).ToJsonString());
                    Console.WriteLine(minecraft.Raw.Libraries.Count);
                    Console.WriteLine(minecraft.Raw.Type);
                    Console.WriteLine(minecraft.Raw.AssetId);
                    Console.WriteLine(minecraft.Raw.InheritsFrom);
                    Console.WriteLine(minecraft.Raw.MainClass);
                    Console.WriteLine(minecraft.Raw.MinecraftArguments);
                    Console.WriteLine(minecraft.Raw.AssetIndexUrl);
                    Console.WriteLine();
                }
            }
        }
    }
}