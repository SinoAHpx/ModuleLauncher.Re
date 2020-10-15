using System;
using ModuleLauncher.Re.Authenticator;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Minecraft.Network;

namespace ModuleLauncher.Re.Test
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            foreach (var s in MojangApi.GetUuidsByNamesAsync(new[] {"AHpxChina", "Ghost"}).GetResult())
            {
                Console.WriteLine(s);
                Console.WriteLine();
            }

            Console.WriteLine(new YggdrasilAuthenticator("AHpx@yandex.com", "asd123,./").AuthenticateAsync().GetResult()
                .Username);

            Console.Read();
        }
    }
}