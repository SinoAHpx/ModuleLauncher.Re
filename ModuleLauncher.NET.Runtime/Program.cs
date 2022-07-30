using System.Text;
using Flurl.Http;
using Manganese.Array;
using Manganese.Text;
using ModuleLauncher.NET.Models.Utils;
using ModuleLauncher.NET.Runtime;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json;


static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}