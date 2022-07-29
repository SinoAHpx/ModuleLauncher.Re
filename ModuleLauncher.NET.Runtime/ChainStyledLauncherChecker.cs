using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Runtime;

public class ChainStyledLauncherChecker
{
    public static async Task CheckAsync()
    {
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
            .WithJava(@"C:\Program Files\Android\jdk\jdk-8.0.302.8-hotspot\jdk8u302-b08\jre\bin\javaw.exe")
            .WithLauncherName("AHpxLauncher")
            .WithMaxMemorySize(4096)
            .WithMinMemorySize(1024)
            .WithFullscreen()
            .LaunchAsync();

        while (!process.ReadOutputLine().IsNullOrEmpty())
        {
            Console.WriteLine(process.ReadOutputLine());
        }
    }
}