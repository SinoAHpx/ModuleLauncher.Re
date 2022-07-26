using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;

MinecraftJsonType.Release.GetDescription().Print();

static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}