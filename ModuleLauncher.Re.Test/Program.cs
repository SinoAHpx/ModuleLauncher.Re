using System;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticator;
using ModuleLauncher.Re.Service.Utils;
using Newtonsoft.Json;

namespace ModuleLauncher.Re.Test
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var ygg = new YggdrasilAuthenticator("AHpx@yandex.com","");

            var re = await ygg.AuthenticateAsync();
            Console.WriteLine(re.Username);
        }
    }
}