using Downloader;
using Flurl.Http;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using Polly;

var version = AnsiConsole.Ask<string>("Which version you want to launch? ");
var rootPath = @"C:\Users\ahpx\Desktop\NewMinecraft\.minecraft";
var mcResolver = new MinecraftResolver(rootPath);
var minecraft = await mcResolver.GetRemoteMinecraftAndToLocalAsync(version);

var libs = minecraft.GetLibraries();
foreach (var library in libs)
{
    AnsiConsole.MarkupLine($"Name: [red]{library.RelativeUrl}[/]");
    AnsiConsole.MarkupLine($"Download: {library.GetDownloadUrl()}");
}

return;
AnsiConsole.MarkupLine("Minecraft grabbed");

if (!minecraft.Tree.Jar.Exists)
{
    AnsiConsole.MarkupLine($"Starting download Minecraft {version}");
    await Policy.Handle<Exception>().RetryForeverAsync().ExecuteAsync(async () =>
    {
        var url = minecraft.GetDownloadUrl();
        await DownloadAsync(url, minecraft.Tree.Jar);
    });

    AnsiConsole.MarkupLine("[green]Completed[/]");
}

var assets = await minecraft.GetAssetsAsync();
foreach (var assetEntry in assets)
{
    if (assetEntry.File.Exists)
    {
        continue;
    }
    AnsiConsole.MarkupLine($"Starting download [red]{assetEntry.Hash}[/]");
    var url = assetEntry.GetDownloadUrl();
    await Policy.Handle<Exception>().RetryAsync(5).ExecuteAsync(async () =>
    {
        await DownloadAsync(url, assetEntry.File);
    });
    
    AnsiConsole.MarkupLine("[green]Completed[/]");

}

AnsiConsole.MarkupLine($"Downloaded {assets.Count} assets in total");

var libraries = minecraft.GetLibraries();
foreach (var libraryEntry in libraries)
{
    if (libraryEntry.File.Exists)
    {
        continue;
    }
    AnsiConsole.MarkupLine($"Starting download [red]{libraryEntry.File.Name}[/]");
    var url = libraryEntry.GetDownloadUrl();
    await Policy.Handle<Exception>().RetryAsync(5).ExecuteAsync(async () =>
    {
        await DownloadAsync(url, libraryEntry.File);
    });

    AnsiConsole.MarkupLine("[green]Completed[/]");
}

AnsiConsole.MarkupLine($"Downloaded {libraries.Count} libraries in total");

AnsiConsole.MarkupLine($"Starting launching {version}");
var process = await minecraft.WithAuthentication("AHpx")
    .WithJava(@"C:\Program Files\Java\jdk1.8.0_321\bin\javaw.exe")
    .WithJava(
        @"C:\Users\ahpx\AppData\Local\Packages\Microsoft.4297127D64EC6_8wekyb3d8bbwe\LocalCache\Local\runtime\java-runtime-beta\windows-x64\java-runtime-beta\bin\javaw.exe")
    .LaunchAsync();

LauncherUtils.Launcher?.GetLaunchArguments(minecraft).Print();

while (!process.ReadOutputLine().IsNullOrEmpty())
{
    Console.WriteLine(process.ReadOutputLine());
}

static async Task DownloadAsync(string url, FileInfo file)
{
    await url.DownloadFileAsync(file.DirectoryName, file.Name);
}

static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}