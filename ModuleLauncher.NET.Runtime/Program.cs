using Manganese.Text;
using ModuleLauncher.NET.Resources;

var mcResolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
foreach (var entry in mcResolver.GetMinecrafts())
{
    Console.WriteLine(entry.Tree.VersionRoot.Name);
}


static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}