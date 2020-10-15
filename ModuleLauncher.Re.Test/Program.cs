using System;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Minecraft.Network;

namespace ModuleLauncher.Re.Test
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            foreach (var mojangServiceStatuse in MojangApi.GetMojangServiceStatusesAsync().GetResult())
            {
                Console.WriteLine(mojangServiceStatuse.Server);
                Console.WriteLine(mojangServiceStatuse.Status);
                Console.WriteLine();
            }

            Console.Read();
        }
    }
}