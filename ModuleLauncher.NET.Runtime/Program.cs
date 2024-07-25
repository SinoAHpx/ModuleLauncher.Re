using Flurl.Http;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using Timer = System.Timers.Timer;

class Program
{
    public async static Task Main()
    {
        // var ms = new MicrosoftAuthenticator
        // {
        //     ClientId = ""
        // };
        // var di = await ms.GetDeviceCodeAsync();
        // di.UserCode.Print();
        // di.VerificationUrl.OpenUrl();
        //
        // var polling = await ms.PollAuthorizationAsync(di);
        // Console.WriteLine(polling);
        // var xbl = await ms.AuthenticateAsync(polling);
        // Console.WriteLine(xbl.ToJsonString());

        var resolver = new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
        var minecraft = resolver.GetMinecraft("1.21");
        var process = await minecraft.WithAuthentication(new AuthenticateResult
            {
            })
            .WithJava("C:\\Program Files\\Eclipse Adoptium\\jre-21.0.3.9-hotspot\\bin\\javaw.exe")
            .LaunchAsync();

        while (await process.ReadOutputLineAsync() is {} op)
        {
            Console.WriteLine(op);
        }
    }
}

static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}