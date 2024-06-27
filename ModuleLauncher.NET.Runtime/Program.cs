using Manganese.Text;
using ModuleLauncher.NET.Mods.Utilities;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using System.Runtime.CompilerServices;
using Manganese.Process;
using Tommy;

const string MinecraftRoot = @"C:\Users\ahpx\AppData\Roaming\.minecraft";
var resolver = new MinecraftResolver(MinecraftRoot);

var mc = resolver.GetMinecraft("fabric-loader-0.15.11-1.21");

var process = await mc.WithAuthentication("AHpx")
    .WithJava(@"C:\Program Files\Eclipse Adoptium\jre-21.0.3.9-hotspot\bin\javaw.exe")
    .WithLauncherName("Latest Version")
    .WithDirectServer("hypixel.net")
    .LaunchAsync();

while (!process.HasExited)
{
    process.ReadOutputLine().Print();
}

static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}