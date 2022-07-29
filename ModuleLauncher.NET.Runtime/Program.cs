using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;
using ModuleLauncher.NET.Utilities;

var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
var minecraft = await resolver.GetRemoteMinecraftAndToLocalAsync("b1.8");

minecraft.GetDownloadUrl(DownloadSource.Bmcl).Print();

static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}