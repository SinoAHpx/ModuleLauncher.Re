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
            Locator.GetMinecraftFileEntities().ForEach(x =>
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine(x.Name);
                
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(Locator.GetMinecraftVersionRoot(x.Name));
                
                Console.WriteLine();
            });
            
            /*var at = new AssetsLocator(Locator);
            at.GetAssets("1.8.9").ForEach(z =>
            {
                Console.WriteLine(z.Link);
            });*/
        }
    }
}