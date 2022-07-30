using Manganese.Text;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json.Linq;

await LauncherChecker.CheckAsync("1.6.4-Forge9.11.1.1345", @"C:\Users\ahpx\AppData\Roaming\.minecraft\.minecraft-legacy");
return;
var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
while (true)
{
    var minecraft = resolver.GetMinecraft(AnsiConsole.Ask<string>("Input a id: "));
    foreach (var assetEntry in AssetsResolver.GetAssets(minecraft))
    {
        Console.WriteLine(assetEntry.RelativeUrl);
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