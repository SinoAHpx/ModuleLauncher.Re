using ModuleLauncher.NET.Runtime;

LibrariesChecker.Check();

static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}