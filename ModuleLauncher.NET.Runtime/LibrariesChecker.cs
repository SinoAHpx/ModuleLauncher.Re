using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Runtime;

public class LibrariesChecker
{
    public static void Check()
    {
        var mcPath = AnsiConsole.Confirm("Use default .minecraft path?")
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft")
            : AnsiConsole.Ask<string>("What is your expected path? ");
        var resolver = new MinecraftResolver(mcPath);
        while (true)
        {
            var mc = resolver.GetMinecraft(AnsiConsole.Ask<string>("Minecraft id: "));
            var libResolver = new LibrariesResolver();
            
            AnsiConsole.MarkupLine($"Minecraft type: [red]{mc.GetMinecraftType()}[/]");

            foreach (var libraryEntry in LibrariesResolver.GetLibraries(mc))
            {
                AnsiConsole.MarkupLine(
                    $"[{(libraryEntry.IsNative ? "red" : "green")}]{libraryEntry.Name}[/], Type: [blue]{libraryEntry.Type}[/]");
            }
        }
    }
}