using System;
using System.Threading.Tasks;
using Masuit.Tools;
using ModuleLauncher.Re.Authenticator;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Minecraft.Locator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Test
{
    internal static class Program
    {
        private static readonly MinecraftLocator Locator = @"C:\Users\ahpx\AppData\Roaming\.minecraft";
        public static void Main(string[] args)
        {
            Locator.GetMinecraftFileEntities().ForEach(x =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Name:{x.Name}");
                Console.WriteLine($"Type:{Locator.GetMinecraftJsonType(x.Name)}");
                
                Console.ForegroundColor = ConsoleColor.Green;
                var lb = new LibrariesLocator(Locator);
                lb.GetLibraries(x.Name).ForEach(z =>
                {
                    Console.WriteLine(z.Name);
                });

                Console.WriteLine();
            });
        }
    }
}