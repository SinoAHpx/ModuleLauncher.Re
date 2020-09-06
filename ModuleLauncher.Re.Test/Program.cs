using System;
using System.Threading.Tasks;
using Masuit.Tools;
using ModuleLauncher.Re.Authenticator;
using ModuleLauncher.Re.Minecraft.Locator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Test
{
    internal static class Program
    {
        private const string McLocation = @"C:\Users\ahpx\AppData\Roaming\.minecraft";
        public static void Main(string[] args)
        {
            MinecraftLocator mc = McLocation;
            var ls = mc.GetMinecrafts();
            ls.ForEach(Console.WriteLine);
        }
    }
}