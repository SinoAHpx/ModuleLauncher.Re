using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModuleLauncher.Re.DataEntities.Enums;
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
        public IEnumerable<MinecraftFileEntity> GetMinecraftFileEntities()
        {
            return Directory.GetDirectories($"{Location}\\versions")
                .Select(x => GetMinecraftFileEntity(Path.GetFileName(x)));
        }
        
        /// <summary>
        /// 获取指定的Minecraft版本
        /// </summary>
        /// <param name="name">版本文件名</param>
        /// <returns></returns>
        public MinecraftFileEntity GetMinecraftFileEntity(string name)
        {
            return new MinecraftFileEntity
            {
                Jar = $"{Location}\\versions\\{name}\\{name}.jar",
                Json = $"{Location}\\versions\\{name}\\{name}.json",
                Native = $"{Location}\\versions\\{name}\\{name}-natives",
                Root = $"{Location}\\versions\\{name}",
                Name = name
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
        internal MinecraftJsonEntity GetMinecraftJsonEntity(string name)
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

        internal MinecraftJsonEntity GetInheritsMinecraftJsonEntity(string name)
        {
            return GetMinecraftJsonEntity(GetMinecraftJsonEntity(name).inheritsFrom);
        }
        
        /// <summary>
        /// 解析所有versions目录下Minecraft的json文件
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<MinecraftJsonEntity> GetMinecraftJsonEntities()
        {
            return GetMinecraftFileEntities().Select(x => GetMinecraftJsonEntity(Path.GetFileName(x.Root)));
        }

        internal MinecraftJsonType GetMinecraftJsonType(string name)
        {
            try
            {
                var entity = GetMinecraftJsonEntity(name);
                
                //判断是不是快照版本
                if (entity.type != "release") return MinecraftJsonType.Vanilla;
                
                //判断是loader还是loadernew
                if (entity.inheritsFrom != null)
                    return Version.Parse(entity.inheritsFrom) < Version.Parse("1.13")
                        ? MinecraftJsonType.Loader
                        : MinecraftJsonType.LoaderNew;
                    
                //判断是vanilla还是modify
                try
                {
                    Version.Parse(entity.id);
                    return MinecraftJsonType.Vanilla;
                }
                catch
                {
                    return MinecraftJsonType.Modify;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"获取Minecraft实体失败\n{e.Message}");
            }
        }
    }
}