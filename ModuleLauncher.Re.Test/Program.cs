using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Masuit.Tools;
using ModuleLauncher.Re.Authenticator;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Minecraft.Locator;
using ModuleLauncher.Re.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Test
{
    internal static class Program
    {
        private static readonly MinecraftLocator Locator = @"C:\Users\ahpx\AppData\Roaming\.minecraft";
        private const string TestJson = @"C:\Users\ahpx\RiderProjects\ModuleLauncher.Re\ModuleLauncher.Re.Test\Resources\TestJsonFile.json";
        
        public static void Main(string[] args)
        {
            var lb = new LibrariesLocator(Locator);
            lb.GetNatives("1.16.2").ForEach(x =>
            {
                Console.WriteLine(x.Path);
            });
        }
    }
}