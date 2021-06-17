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
            Console.WriteLine(new MicrosoftAuthenticator().GetMicrosoftAuthorizeToken());
        }
    }
}