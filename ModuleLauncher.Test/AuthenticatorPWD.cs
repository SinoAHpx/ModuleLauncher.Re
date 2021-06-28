using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Test
{
    public static class AuthenticatorPWD
    {
        public class PWD
        {
            public string Account { get; set; }
            public string Password { get; set; }
        }

        public static PWD GetPWD(string filePath = "/home/ahpx/公共/CodeBase/account.json")
        {
            return JsonConvert.DeserializeObject<PWD>(File.ReadAllText(filePath));
        }
    }
}