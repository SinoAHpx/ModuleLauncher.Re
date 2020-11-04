using System;
using System.Linq;
using AHpx.ModuleLauncher.Locators;

namespace AHpx.ModuleLauncher
{
    public class Entrance
    {
        public static void Main(string[] args)
        {
            var lo = new MinecraftLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
            lo.GetMinecrafts(false).ToList().ForEach(Console.WriteLine);
        }
    }
    
    public class Test
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public bool Gender { get; set; }
        internal bool Family { get; set; }
        private int Grade { get; set; } = 99;

        public override string ToString()
        {
            var props = this.GetType().GetProperties();
            foreach (var info in props)
            {
                if (info.GetAccessors(false)[0].IsPublic)
                {
                    Console.WriteLine(info.Name);
                }
            }

            return "END";
        }
    }
}