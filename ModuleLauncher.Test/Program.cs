using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Locators;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var lct = new MinecraftLocator(@"D:\Minecraft\Solution1\.minecraft");
            
            var version = lct.GetMinecrafts();
            
            foreach (var minecraft in version)
            {
                var arr = minecraft.Json.Arguments?.Fetch("game").ToJArray();

                if (minecraft.Locality.Version.Name == "1.14.4")
                {
                    if (arr != null)
                    {
                        var a2 = new List<string>();

                        foreach (var token in arr)
                        {
                            if (token.Type == JTokenType.String)
                            {
                                a2.Add(token.ToString());
                            }
                        }

                        Console.WriteLine(a2.ToJsonString());
                    }
                }
                
            }
            Console.WriteLine();
        }
    }
}