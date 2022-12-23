using ModuleLauncher.NET.Mods.Utilities;
using Tommy;

var info = await ModUtils.GetModInfoAsync(@"C:\Users\ahpx\Downloads\jei-1.19.2-forge-11.5.0.297.jar");
info.Print();
    
static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}