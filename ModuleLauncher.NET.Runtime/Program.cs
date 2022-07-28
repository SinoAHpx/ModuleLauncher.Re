using System.Runtime.InteropServices;
using ModuleLauncher.NET;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;

var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
var minecraft = resolver.GetMinecraft("1.19-forge-41.0.110");

var launcher = new Launcher(resolver, new LauncherConfig
{
    Authentication = "ahpx",
    Fullscreen = false,
    Javas = new List<MinecraftJava>
    {
        new()
        {
            Version = 17,
            Executable = new FileInfo(@"C:\Program Files\Eclipse Adoptium\jdk-17.0.2.8-hotspot\bin\javaw.exe")
        }
    },
    LauncherName = "AHpxLauncher",
    MaxMemorySize = 2048
});

var command = launcher.GetLaunchArguments(minecraft);
command.Print();

static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}