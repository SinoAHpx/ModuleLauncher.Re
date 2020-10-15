using System;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Minecraft.Network;

namespace ModuleLauncher.Re.Test
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(MojangApi.GetUuid("AHpxChina").GetResult().Name);
            Console.WriteLine(MojangApi.GetUuid("AHpxChina").GetResult().Uuid);

            Console.Read();
        }
    }
}