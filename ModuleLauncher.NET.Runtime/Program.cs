using CliWrap;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;

var locator = new MinecraftResolver("""C:\Users\ahpx\AppData\Roaming\.minecraft""");
var toLaunch = locator.GetMinecraft("1.19.2");
var launcher = new Launcher
{
    LauncherConfig = new LauncherConfig
    {
        Javas = new List<MinecraftJava>
        {
            MinecraftJava.Of("""C:\Program Files\Eclipse Adoptium\jdk-17.0.4.101-hotspot\bin\javaw.exe""")!,
            MinecraftJava.Of("""C:\Program Files\Eclipse Adoptium\jdk-8.0.345.1-hotspot\jre\bin\javaw.exe""")!
        },
        Authentication = "AHpx"
    }
};

var buffer = await launcher.LaunchAsync(toLaunch, PipeTarget.ToDelegate(Console.WriteLine));

Console.WriteLine($"Exited with code {buffer.ExitCode}");


static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}