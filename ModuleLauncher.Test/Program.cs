using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Launcher;
using ModuleLauncher.Re.Locators;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var foo = new Launcher(@"C:\Users\ahpx\AppData\Roaming\.minecraft")
            {
                Authentication = "AHpx",
                Java = @"C:\Program Files\Java\jre1.8.0_291\bin\javaw.exe",
                MaximumMemorySize = 1024,
                LauncherName = "Ahpx",
                Fullscreen = false
            };

            var re = await foo.Launch("1.8.9");

            re.Exited += (_, _) =>
            {
                Console.WriteLine("Game exited with code " + re.ExitCode);
            };

            while (await re.StandardOutput.ReadLineAsync() != null)
            {
                Console.WriteLine(await re.StandardOutput.ReadLineAsync());
            }

            await re.WaitForExitAsync();
        }
    }
}