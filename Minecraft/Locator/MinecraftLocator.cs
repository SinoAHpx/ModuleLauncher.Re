using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;
using Newtonsoft.Json;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    //head
    public partial class MinecraftLocator
    {
        public string Location { get; set; }

        public MinecraftLocator(string location = ".\\.minecraft")
        {
            Location = location;
        }
        
        public static implicit operator MinecraftLocator(string location)
        {
            return new MinecraftLocator(location);
        }
        
        public static implicit operator string(MinecraftLocator location)
        {
            return location.Location;
        }
    }
    
    //exposed
    public partial class MinecraftLocator
    {
        /// <summary>
        /// 获取versions目录下所有的Minecraft版本
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
        
        /// <summary>
        /// 获取指定的Minecraft版本
        /// </summary>
        /// <param name="name">版本文件名</param>
        /// <returns></returns>
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
        /// <summary>
        /// 解析指定Minecraft版本的json文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal MinecraftJsonEntity GetMinecraftEntity(string name)
        {
            try
            {
                var json = File.ReadAllText($"{Location}\\versions\\{name}\\{name}.json");
                return JsonConvert.DeserializeObject<MinecraftJsonEntity>(json);
            }
            catch (Exception e)
            {
                throw new Exception($"json文件不存在\n{e.Message}");
            }
        }
        
        /// <summary>
        /// 解析所有versions目录下Minecraft的json文件
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<MinecraftJsonEntity> GetMinecraftEntities()
        {
            return GetMinecrafts().Select(x => GetMinecraftEntity(Path.GetFileName(x.Root)));
        }
    }
}