using System;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Minecraft.Network;

namespace ModuleLauncher.Re.Test
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            foreach (var name in MojangApi.GetHistoryNamesAsync(MojangApi.GetUuidAsync("AHpxChina").GetResult().Uuid)
                .GetResult())
            {
                Console.WriteLine(name.Name);
                Console.WriteLine(name.ChangedAt);
                Console.WriteLine();
            }

            Console.Read();
        }
    }
}