using ModuleLauncher.NET.Runtime;

await Authentication.DoAuthAsync(); 

static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}