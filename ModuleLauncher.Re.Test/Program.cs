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
            var lv = new LibrariesLocator(Locator);
            lv.GetLibraries("1.16.2").ForEach(entity => Console.WriteLine(entity.Name));
            Console.WriteLine("============================================================");
            lv.GetLibrariesss("1.16.2").ForEach(entity => Console.WriteLine(entity.Name));
        }
    }
}