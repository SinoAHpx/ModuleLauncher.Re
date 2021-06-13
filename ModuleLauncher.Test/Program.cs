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
            var obj = JObject.Parse(File.ReadAllText(@"C:\Users\ahpx\Desktop\account.json"));
            
            var ac = new MojangAuthenticator
            {
                Account = obj.Fetch("Account"),
                Password = "lwidkjmawd",
                ClientToken = null
            };

            var re = await ac.Authenticate();

            Console.WriteLine(re.ToJsonString());
        }
    }
}