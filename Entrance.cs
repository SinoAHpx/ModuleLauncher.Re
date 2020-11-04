using System;
using System.Linq;
using AHpx.ModuleLauncher.Locators;

namespace AHpx.ModuleLauncher
{
    public class Entrance
    {
        public static void Main(string[] args)
        {
            var lo = new MinecraftLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
            lo.GetMinecrafts(false).ToList().ForEach(x =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Version:");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(x.File.Version.Name);
                
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Type:");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine(x.Type);
                Console.WriteLine();
            });
        }
    }
}