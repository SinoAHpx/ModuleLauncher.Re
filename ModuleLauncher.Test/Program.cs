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
                Account = AuthenticatorPWD.GetPWD().Account,
                Password = AuthenticatorPWD.GetPWD().Password
            };

            var re = await ac.Authenticate();
            Console.WriteLine(re.ToJsonString());
            Console.WriteLine($"R1: {await ac.Validate(re.AccessToken)}");

            var re2 = await ac.Refresh(re.AccessToken, re.ClientToken);

            Console.WriteLine(re2.ToJsonString());

            Console.WriteLine($"R1: {await ac.Validate(re.AccessToken)}");// or you can use extension methods like re.Validate()

            Console.WriteLine($"R2: {await ac.Validate(re2.AccessToken)}");

            await ac.SignOut();
            
            Console.WriteLine($"R2: {await ac.Validate(re2.AccessToken)}");

            var re3 = await ac.Authenticate();
            Console.WriteLine(re3.ToJsonString());

            Console.WriteLine($"R3: {await ac.Validate(re3.AccessToken)}");
            await ac.Invalidate(re3.AccessToken, re3.ClientToken);

            Console.WriteLine($"R3: {await ac.Validate(re3.AccessToken)}");

        }
    }
}