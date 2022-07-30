using System.Text;
using Flurl.Http;
using Manganese.Text;

var response = await "https://sessionserver.mojang.com/session/minecraft/profile/db9c8b5f84ef493ebc58d218e2e0f007"
    .GetStringAsync();

var base64 = response.FetchJToken("properties").First.Fetch("value");
var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64));

decoded.Print();

static class RuntimeUtils
{
    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);
        
        return t;
    }
}