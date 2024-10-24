﻿using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Resources;

namespace ModuleLauncher.NET.Runtime;

public class LauncherChecker
{
    public static async Task CheckAsync(string? defaultVersion = null, string? defaultWorkingDirectory = null)
    {
        var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
        var launcher = new Launcher.Launcher(resolver, new LauncherConfig
        {
            Authentication = "ahpx",
            Fullscreen = false,
            Javas = new List<MinecraftJava>
            {
                new()
                {
                    Version = 17,
                    Executable = new FileInfo(@"C:\Program Files\Eclipse Adoptium\jdk-17.0.2.8-hotspot\bin\javaw.exe")
                },
                new()
                {
                    Version = 8,
                    Executable =
                        new FileInfo(
                            @"C:\Program Files\Android\jdk\jdk-8.0.302.8-hotspot\jdk8u302-b08\jre\bin\javaw.exe")
                },
                new()
                {
                    Version = 16,
                    Executable =
                        new FileInfo(
                            @"C:\Users\ahpx\AppData\Local\Packages\Microsoft.4297127D64EC6_8wekyb3d8bbwe\LocalCache\Local\runtime\java-runtime-alpha\windows-x64\java-runtime-alpha\bin\javaw.exe")
                }
            },
            LauncherName = "AHpxLauncher",
            MaxMemorySize = 2048
        });
        
        while (true)
        {
            defaultVersion ??= AnsiConsole.Ask<string>("Input a [red bold]version[/]: ");
            defaultWorkingDirectory ??= AnsiConsole.Ask<string>("Input an [red]absolute path[/]: ");
            
            var minecraft = resolver.GetMinecraft(defaultVersion, defaultWorkingDirectory);

            var command = launcher.GetLaunchArguments(minecraft);
            command.Print();

            var process = await launcher.LaunchAsync(minecraft);
            while (!(await process.ReadOutputLineAsync()).IsNullOrEmpty())
            {
                Console.WriteLine(await process.ReadOutputLineAsync());
            }
            
            if (!AnsiConsole.Confirm("Again? "))
            {
                return;
            }
        }
    }
}