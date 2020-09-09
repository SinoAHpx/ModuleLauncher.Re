using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Masuit.Tools;
using ModuleLauncher.Re.Authenticator;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Launcher;
using ModuleLauncher.Re.Minecraft.Locator;
using ModuleLauncher.Re.Minecraft.Network;
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
            var la = new LauncherArguments
            {
                Authentication = "awd",
                
                MaxMemorySize = "6G",
                
                LauncherName = "AHpxLauncher",
                JvmArgument = "-noverify",
                MinecraftLocator = Locator
            };

            var lc = new LauncherCore
            {
                LauncherArguments = la,
                JavaPath = @"C:\Program Files\Java\jdk1.8.0_241\bin\javaw.exe"
            };

            var pro = lc.Launch("fabric-loader-0.9.3+build.207-1.14.4");
            while (pro.StandardOutput.ReadLine() != null)
            {
                Console.WriteLine(pro.StandardOutput.ReadLine());
            }
        }
    }
}