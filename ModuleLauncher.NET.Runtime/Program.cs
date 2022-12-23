using ModuleLauncher.NET.Mods.Utilities;

var info = await ModUtils.GetModInfoAsync(@"C:\Users\ahpx\Desktop\mods\1.8.9\jei_1.8.9-2.28.18.187.jar");
info.Print();

static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}