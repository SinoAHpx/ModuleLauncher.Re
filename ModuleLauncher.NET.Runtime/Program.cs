using System.Diagnostics;
using Flurl.Http;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;
using ModuleLauncher.NET.Utilities;

var authenticator = new MicrosoftAuthenticator
{
    ClientId = "1b0c7f80-247c-4101-bdc9-a98c479471a4"
};
Console.WriteLine(authenticator.LoginUrl);
var code = AnsiConsole.Ask<string>("Redirect url: ").ExtractCode();
authenticator.Code = code;

var authentication = await authenticator.AuthenticateAsync();
Console.WriteLine($"Authentication: {authentication.ToJsonString()}");

var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
var process = await resolver.GetMinecraft("1.19")
    .WithAuthentication(authentication)
    .WithJava(@"C:\Program Files\Eclipse Adoptium\jdk-17.0.2.8-hotspot\bin\javaw.exe")
    .LaunchAsync();

while (!process.ReadOutputLine().IsNullOrEmpty())
{
    Console.WriteLine(process.ReadOutputLine());
}



static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}