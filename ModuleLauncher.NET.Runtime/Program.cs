using Manganese.Text;
using ModuleLauncher.NET.Mods.Utilities;
using Tommy;

while (true)
{
    var info = await ModUtils.GetModInfoAsync(AnsiConsole.Ask<string>("Mod [red]path[/]:").Trim('"'));
    info.ToJsonString().Print();
}

    
static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}