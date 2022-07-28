using System.Runtime.InteropServices;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;

var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
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
        },
        new()
        {
            Version = 8,
            Executable =
                new FileInfo(@"C:\Program Files\Android\jdk\jdk-8.0.302.8-hotspot\jdk8u302-b08\jre\bin\javaw.exe")
        }
    },
    LauncherName = "AHpxLauncher",
    MaxMemorySize = 2048
});

while (true)
{
    var id = AnsiConsole.Ask<string>("Input a [red bold]version[/]: ");
    var minecraft = resolver.GetMinecraft(id);
    
    var command = launcher.GetLaunchArguments(minecraft);
    command.Print();

    var process = await launcher.LaunchAsync(minecraft);
    while (!process.ReadOutputLine().IsNullOrEmpty())
    {
        Console.WriteLine(process.ReadOutputLine());
    }
}


static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}