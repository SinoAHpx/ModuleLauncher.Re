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
            Console.WriteLine("net.java.dev.jna:platform:3.4.0".ToLibraryPath(true));
        }
    }
}