using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Runtime;

public class DownloadUrlChecker
{
    public async Task CheckMcAsync()
    {
        var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
        var minecraft = await resolver.GetRemoteMinecraftAndToLocalAsync("b1.8");

        minecraft.GetDownloadUrl(DownloadSource.Bmcl).Print();
    }
}