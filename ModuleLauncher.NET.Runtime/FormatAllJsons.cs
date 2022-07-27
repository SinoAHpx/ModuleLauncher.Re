using Manganese.IO;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.NET.Runtime;

public static class FormatAllJsons
{
    public static void Format(string minecraftPath)
    {
        minecraftPath = minecraftPath.TrimEnd(Path.DirectorySeparatorChar);
        var minecraftVersions = new DirectoryInfo($"{minecraftPath}{Path.DirectorySeparatorChar}versions");
        foreach (var versionDir in minecraftVersions.GetDirectories())
        {
            foreach (var versionFile in versionDir.GetFiles())
            {
                if (versionFile.Name.EndsWith(".json"))
                {
                    AnsiConsole.MarkupLine($"Now working on [red]{versionFile.Name}[/]");
                    versionFile.WriteAllText(JObject.Parse(versionFile.ReadAllText()).ToString());
                }
            }
        }
    }
}