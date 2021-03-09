using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Authenticators;
using AHpx.ModuleLauncher.Data.Downloaders;
using AHpx.ModuleLauncher.Downloaders;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using AHpx.ModuleLauncher.Utils.Network;

namespace AHpx.ModuleLauncher
{
    public static class Entrance
    {
        public static async Task Main(string[] args)
        {
            var ms = new MicrosoftAuthenticator("M.R3_BAY.208b1c40-517e-7865-1b72-9d6eaa017fbd");
            // var re = await ms.Authenticate();
            //
            // Console.WriteLine(re.Name);
            // Console.WriteLine(re.Uuid);
            // Console.WriteLine(re.AccessToken);
            // Console.WriteLine(re.Verified);

            Console.WriteLine(await ms.CheckHaveMinecraft("eyJhbGciOiJIUzI1NiJ9.eyJ4dWlkIjoiMjUzNTQ0MDM0MjkwNzc3NyIsInN1YiI6IjY4ZGNjMzA1LTIyNTItNDMzZS1iYzQ5LWE2ODI3NGMzOTliYyIsIm5iZiI6MTYxNDg2NzQ2OSwicm9sZXMiOltdLCJpc3MiOiJhdXRoZW50aWNhdGlvbiIsImV4cCI6MTYxNDk1Mzg2OSwiaWF0IjoxNjE0ODY3NDY5fQ.cLNnisLHr0kO8w1vI79t49EBt5CNMxCRKs2HMuiXXd0"));
        }

        private static void Output<T>(this IEnumerable<T> ex)
        {
            foreach (var x1 in ex)
            {
                Console.WriteLine(x1);
            }
        }
    }
}