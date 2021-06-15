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
            var mc = new MicrosoftAuthenticator
            {
                Code = "M.R3_BAY.cf2e856b-9d44-9f86-2905-a999e02766c5"
            };

            var result = await mc.Authenticate();

            Console.WriteLine(result.ToJsonString());
        }
    }
}