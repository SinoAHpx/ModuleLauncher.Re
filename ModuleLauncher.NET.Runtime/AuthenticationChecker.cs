using Manganese.Text;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Utilities;
using Spectre.Console;

namespace ModuleLauncher.NET.Runtime;

public class AuthenticationChecker
{
    public static async Task DoAuthAsync()
    {
        // var authenticator = new MicrosoftAuthenticator
        // {
        //     ClientId = "1b0c7f80-247c-4101-bdc9-a98c479471a4",
        // };
        //
        // Console.WriteLine(authenticator.LoginUrl);
        //
        // var code = AnsiConsole.Ask<string>("What is your [red]code[/]? ");
        // authenticator.Code = code.ExtractCode();
        //
        // var result = await authenticator.AuthenticateAsync();
        // var haveMc = await authenticator.CheckOwnershipAsync(result.AccessToken);
        // AnsiConsole.MarkupLine($"This user {(haveMc ? "have" : "don't have")} a minecraft");
        //
        // AnsiConsole.MarkupLine(result.ToJsonString().EscapeMarkup());
        //
        // AnsiConsole.MarkupLine("[red]refreshing...[/]");
        //
        // var refreshToken = result.RefreshToken;
        // var newResult = await authenticator.RefreshAuthenticateAsync(refreshToken);
        //
        // AnsiConsole.MarkupLine($"new result: {newResult.ToJsonString()}");
    }
}