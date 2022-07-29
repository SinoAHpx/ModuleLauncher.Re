using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;

var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");


static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}