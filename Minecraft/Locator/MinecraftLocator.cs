using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    //head
    public partial class MinecraftLocator
    {
        public string Location { get; set; }

        public MinecraftLocator(string location = ".\\")
        {
            Location = location;
        }

        public static implicit operator MinecraftLocator(string location)
        {
            return new MinecraftLocator(location);
        }
    }
    
    public partial class MinecraftLocator
    {
        public IEnumerable<string> GetMinecrafts(bool path = false)
        {
            var re = Directory.GetDirectories(Location);
            return path ? re : re.Select(Path.GetFileName);
        }
    }
    
    public partial class MinecraftLocator
    {
        
    }
}