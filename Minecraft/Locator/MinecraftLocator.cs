using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;

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
    
    //exposed
    public partial class MinecraftLocator
    {
        /// <summary>
        /// 获取versions目录下所有的Minecraft版本文件夹
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MinecraftFileEntity> GetMinecrafts()
        {
            return Directory.GetDirectories($"{Location}\\versions").Select(s => new MinecraftFileEntity
            {
                Jar = $"{s}\\{Path.GetFileName(s)}.jar",
                Json = $"{s}\\{Path.GetFileName(s)}.json",
                Native = $"{s}\\{Path.GetFileName(s)}-natives",
                Root = s
            });
        }
        
        public MinecraftFileEntity GetMinecraft(string name)
        {
            return new MinecraftFileEntity
            {
                Jar = $"{Location}\\versions\\{name}\\{name}.jar",
                Json = $"{Location}\\versions\\{name}\\{name}.json",
                Native = $"{Location}\\versions\\{name}\\{name}-natives",
                Root = $"{Location}\\versions\\{name}"
            };
        }
    }
    
    //inside
    public partial class MinecraftLocator
    {
        internal MinecraftJsonEntity GetMinecraftEntity()
        {
            return null;
        }
        
        internal IEnumerable<MinecraftJsonEntity> GetMinecraftEntities(bool path = false)
        {
            var re = Directory.GetDirectories($"{Location}\\versions");
            return null;
        }
    }
}