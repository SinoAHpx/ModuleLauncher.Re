using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Runtime;

public class AssetsChecker
{
    public static void Check()
    {
        var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
        while (true)
        {
            var minecraft = resolver.GetMinecraft(AnsiConsole.Ask<string>("Input a id: "));
            foreach (var assetEntry in AssetsResolver.GetAssets(minecraft))
            {
                AnsiConsole.MarkupLine($"[red]{assetEntry.File}[/]");
                AnsiConsole.MarkupLine($"[blue]{assetEntry.GetDownloadUrl()}[/]");
            }
        }
    }
}