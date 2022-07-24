using Manganese.Array;
using Manganese.Text;

namespace ModuleLauncher.NET.Runtime;

public static class LauncherProfileWriter
{
    public static async Task<string> ReadProfilesJsonAsync()
    {
        var path = new DirectoryInfo(@$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\.minecraft");

        if (!path.Exists)
        {
            throw new InvalidOperationException("Default Minecraft path does not exist");
        }

        var file = new FileInfo($"{path}\\launcher_profiles.json");

        if (!file.Exists)
        {
            throw new InvalidOperationException("You have to run launcher for at least once");
        }

        return await File.ReadAllTextAsync(file.FullName);
    }

    public static async Task<List<string>> GeneratePrpfilesAsync()
    {
        var versions = await McDebuggingPack.GetVersionArrayAsync();
        if (versions == null)
        {
            throw new InvalidOperationException("Failed to request versions");
        }

        //use the value below, because this is C# 11
        //this variable exists due to stable Rider currently don't support syntax of C# 11
        //and I don't want it to "error" all the time
        var boilerplate = "";
        
        // var boilerplate = """
        //     "#GUID#" : {
        //       "created" : "#TIME#",
        //       "icon" : "Furnace",
        //       "lastUsed" : "#TIME#",
        //       "lastVersionId" : "#ID#",
        //       "name" : "#ID#",
        //       "type" : "custom"
        //     }
        // """;

        var versionIds = versions
            .Where(x => x.Fetch("type") == "release")
            .Select(x => x.Fetch("id")!).ToList();
        var toBeAdd = new List<string>();
        foreach (var versionId in versionIds)
        {
            var meta = boilerplate
                .Replace("#GUID#", Guid.NewGuid().ToString("N"))
                .Replace("#TIME#", DateTime.Now.ToString("O"))
                .Replace("#ID#", versionId);

            toBeAdd.Add(meta);
        }

        return toBeAdd;
    }
}