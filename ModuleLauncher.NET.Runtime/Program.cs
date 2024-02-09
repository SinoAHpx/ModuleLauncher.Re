using Manganese.Text;
using ModuleLauncher.NET.Mods.Utilities;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using System.Runtime.CompilerServices;
using Tommy;

const string MinecraftRoot = @"C:\Users\ahpx\AppData\Roaming\.minecraft";
var resolver = new MinecraftResolver(MinecraftRoot);

var mc = resolver.GetMinecraft("1.20.4");

await mc.WithAuthentication("AHpx")
    .WithJava(@"C:\Users\ahpx\AppData\Local\Packages\Microsoft.4297127D64EC6_8wekyb3d8bbwe\LocalCache\Local\runtime\java-runtime-gamma\windows-x64\java-runtime-gamma\bin\javaw.exe")
    .WithLauncherName("Latest Version")
    .LaunchAsync(pipeTarget: CliWrap.PipeTarget.ToDelegate(s =>
    {
        Console.WriteLine(s);
    }));

static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}