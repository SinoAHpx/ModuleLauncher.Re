using System;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticator;
using ModuleLauncher.Re.Service.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Test
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var of = new OfflineAuthenticator("AHpx");
            var rs = of.Authenticate();
            Console.WriteLine(JObject.Parse(JsonConvert.SerializeObject(rs)));
        }
    }
}