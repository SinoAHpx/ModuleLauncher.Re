using Flurl.Http;
using Manganese.Text;
using ModuleLauncher.NET.Runtime;

//https://login.live.com/oauth20_authorize.srf?client_id=1b0c7f80-247c-4101-bdc9-a98c479471a4&response_type=code&redirect_uri=https://login.live.com/oauth20_desktop.srf&scope=XboxLive.signin%20offline_access
Console.Write("Waiting for: ");
var code = Console.ReadLine()!.SubstringBetween("code=", "&lc");
Console.WriteLine($"Code is: {code}");

var url = "https://login.live.com/oauth20_token.srf";
var response = await url.PostUrlEncodedAsync(new
{
    grant_type = "authorization_code",
    code,
    client_id = Credentiality.ReadCredential<OAuthInfo>().ClientId,
    redirect_uri = "https://login.live.com/oauth20_desktop.srf"
});

var str = await response.GetStringAsync();
Console.WriteLine(str);