using ModuleLauncher.NET;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;

var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
var minecraft = resolver.GetMinecraft("1.16.4");

var launcher = new Launcher(resolver, new LauncherConfig());
var command = launcher.GetLaunchCommand(minecraft);
command.Print();

static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}