using Manganese.Array;
using Manganese.Text;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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



static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}