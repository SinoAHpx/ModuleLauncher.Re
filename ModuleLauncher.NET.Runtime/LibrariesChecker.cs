using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Runtime;

public class LibrariesChecker
{
    public static void Check()
    {
        while (true)
        {
            var jsonPath = AnsiConsole.Ask<string>("Input your [red]json path[/]: ");
            var json = File.ReadAllText(jsonPath);
            var mj = JsonConvert.DeserializeObject<MinecraftJson>(json);
            var mc = new MinecraftEntry
            {
                Json = mj
            };
            var resolver = new LibrariesResolver
            {
                Minecraft = mc
            };

            AnsiConsole.MarkupLine($"Minecraft type: [red]{mc.Json.GetMinecraftType()}[/]");

            foreach (var libraryEntry in resolver.GetLibraries())
            {
                AnsiConsole.MarkupLine($"[{(libraryEntry.IsNative ? "red" : "green")}]{libraryEntry.Name}[/]");
            }
        }
    }
}