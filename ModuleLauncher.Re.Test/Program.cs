using System;
using System.Threading.Tasks;
using ModuleLauncher.Re.Service.Utils;
using Newtonsoft.Json;

namespace ModuleLauncher.Re.Test
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            /*
             * thaotrang.hoang@gmail.com
             * starfl0wer
             */

            var json = JsonConvert.SerializeObject(new
            {
                agent = new
                {
                    name = "Minecraft",
                    version = 1
                },
                username = "thaotrang.hoang@gmail.com",
                password = "aaa",
                clientToken = ""
            });
            var re = await HttpHelper.PostHttpAsync("https://authserver.mojang.com/authenticate", json);
            Console.WriteLine(re.Content);
        }
    }
}