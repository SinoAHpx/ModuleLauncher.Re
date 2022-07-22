using Flurl.Http;
using Manganese.Text;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace ModuleLauncher.NET.Runtime;

public static class McDebuggingPack
{
    public const string Manifest = "http://launchermeta.mojang.com/mc/game/version_manifest.json"; 
    
    public static async Task<JArray?> GetVersionArrayAsync()
    {
        var json = await Manifest.GetStringAsync();
        return json.Fetch("versions")?.ToJArray();
    }

    public static async Task DownloadSingleAsync(string url, string path)
    {
        path = path.Trim().TrimEnd('\\');
        var versionJson = await url.GetStringAsync();
        if (versionJson == null)
        {
            throw new InvalidOperationException("Failed to retrieve mc json");
        }
        
        var mcId = versionJson.Fetch("id");
        AnsiConsole.MarkupLine($"Working on [green]{mcId}[/]");

        if (mcId == null)
        {
            throw new InvalidOperationException("Minecraft json corrupted");
        }

        var cachePath = new DirectoryInfo($"{path}\\{mcId}");

        if (!cachePath.Exists)
        {
            cachePath.Create();
        }

        versionJson = versionJson.ToJObject().ToString();
        
        await File.WriteAllTextAsync($"{cachePath}\\{mcId}.json", versionJson);

        AnsiConsole.MarkupLine($"[green]Successfully wrote {mcId}.json[/]");
    }

    public static async Task DownloadAsync(string path)
    {
        var cacheDir = new DirectoryInfo(path);
        if (!cacheDir.Exists)
        {
            cacheDir.Create();
        }

        var versions = await GetVersionArrayAsync();
        if (versions == null)
        {
            throw new InvalidOperationException("Failed to retrieve versions");
        }

        var urls = versions
            .Select(j => j.Fetch("url"))
            .Where(s => s != null)
            .Select(s => s!)
            .ToList();
        
        foreach (var url in urls)
        {
            await DownloadSingleAsync(url, path);
        }
    }
}