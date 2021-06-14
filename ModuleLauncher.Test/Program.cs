using System;
using System.IO;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var ac = new MojangAuthenticator
            {
                Account = "AHpx@yandex.com",
                Password = "asd123,./"
            };

            var re = await ac.Authenticate();
            Console.WriteLine(re.ToJsonString());

            var re2 = await ac.Refresh(re.AccessToken, re.ClientToken);

            Console.WriteLine(re2.ToJsonString());
        }
    }
}