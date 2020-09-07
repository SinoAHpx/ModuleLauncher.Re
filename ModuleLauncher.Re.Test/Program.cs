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
        public static void Main(string[] args)
        {
            /*Locator.GetMinecraftFileEntities().ForEach(x =>
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Name:{x.Name}");

                Console.ForegroundColor = ConsoleColor.Green;
                var lb = new LibrariesLocator(Locator);
                lb.GetLibraries(x.Name).ForEach(z =>
                {
                    Console.WriteLine(z.Name);
                });

                Console.WriteLine();
            });*/
            var lb = new LibrariesLocator(Locator,MinecraftDownloadSource.Mojang);
            /*lb.GetLibraries("1.16.2").ForEach(x =>
            {
                Console.WriteLine(x.Path);
            });*/

            lb.GetLibraries("1.16.2").ForEach(x =>
            {
                Console.WriteLine(x.Path);
            });
            Console.WriteLine();
            CollectionHelper.ExcludeRepeatV2(lb.GetLibraries("1.16.2")).ForEach(x =>
            {
                Console.WriteLine(x.Path);
            });
        }
    }
}