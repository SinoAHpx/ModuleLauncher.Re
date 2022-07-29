using System.Diagnostics;
using Flurl.Http;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;
using ModuleLauncher.NET.Utilities;

LibrariesChecker.Check();

static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}