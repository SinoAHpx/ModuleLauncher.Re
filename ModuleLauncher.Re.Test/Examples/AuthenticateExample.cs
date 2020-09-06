using System;
using ModuleLauncher.Re.Authenticator;
using Newtonsoft.Json;

namespace ModuleLauncher.Re.Test.Examples
{
    public class AuthenticateExample
    {
        public static async void Execute(string email,string password)
        {
            var ygg = new YggdrasilAuthenticator(email, password, "112233");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Authenticate==============");
            var re = await ygg.AuthenticateAsync();
            
            Console.WriteLine($"Username:{re.Username}");
            Console.WriteLine($"Access Token:{re.AccessToken}");
            Console.WriteLine($"Client Token:{re.ClientToken}");
            Console.WriteLine($"Uuid:{re.Uuid}");
            Console.WriteLine($"Error:{re.Error}");
            Console.WriteLine($"Error Messages:{re.ErrorMessage}");
            Console.WriteLine($"Verify Result:{re.Verified}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Json Response Text:\n{JsonConvert.SerializeObject(re)}");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Refresh==============");
            var rRe = await ygg.RefreshAsync(re.AccessToken);
            Console.WriteLine($"Username:{rRe.Username}");
            Console.WriteLine($"Access Token:{rRe.AccessToken}");
            Console.WriteLine($"Client Token:{rRe.ClientToken}");
            Console.WriteLine($"Uuid:{rRe.Uuid}");
            Console.WriteLine($"Error:{rRe.Error}");
            Console.WriteLine($"Error Messages:{rRe.ErrorMessage}");
            Console.WriteLine($"Verify Result:{rRe.Verified}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Json Response Text:\n{JsonConvert.SerializeObject(rRe)}");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Validate==============");
            Console.WriteLine($"Validate Before:{await ygg.ValidateAsync(re.AccessToken)}");
            Console.WriteLine($"Validate After:{await ygg.ValidateAsync(rRe.AccessToken)}");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalidate==============");
            await ygg.InvalidateAsync(rRe.AccessToken, rRe.ClientToken);
            Console.WriteLine($"Validate After:{await ygg.ValidateAsync(rRe.AccessToken)}");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("SignOut==============");
            ygg = new YggdrasilAuthenticator("AHpx@yandex.com","asd123,./","445566");
            re = await ygg.AuthenticateAsync();
            
            Console.WriteLine($"Username:{re.Username}");
            Console.WriteLine($"Access Token:{re.AccessToken}");
            Console.WriteLine($"Client Token:{re.ClientToken}");
            Console.WriteLine($"Uuid:{re.Uuid}");
            Console.WriteLine($"Error:{re.Error}");
            Console.WriteLine($"Error Messages:{re.ErrorMessage}");
            Console.WriteLine($"Verify Result:{re.Verified}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Json Response Text:\n{JsonConvert.SerializeObject(re)}");
            
            await ygg.SignOutAsync();
            Console.WriteLine($"Validate:{await ygg.ValidateAsync(re.AccessToken)}");
        }
    }
}