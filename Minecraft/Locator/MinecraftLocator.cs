using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;
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

        public override string ToString()
        {
            return Location;
        }
    }
    
    //exposed
    public partial class MinecraftLocator
    {
        /// <summary>
        /// 获取versions目录下所有的Minecraft版本
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MinecraftFileEntity> GetMinecraftFileEntities() => Directory
            .GetDirectories($"{Location}\\versions")
            .Select(x => GetMinecraftFileEntity(x.GetFileName()));

        /// <summary>
        /// 获取指定的Minecraft版本
        /// </summary>
        /// <param name="name">版本文件名</param>
        /// <returns></returns>
        public MinecraftFileEntity GetMinecraftFileEntity(string name) => new MinecraftFileEntity
        {
            Jar = $"{Location}\\versions\\{name}\\{name}.jar",
            Json = $"{Location}\\versions\\{name}\\{name}.json",
            Native = $"{Location}\\versions\\{name}\\{name}-natives",
            Root = $"{Location}\\versions\\{name}",
            Name = name
        };
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

        /// <summary>
        /// 解析loader类型客户端继承的MinecraftJson
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal MinecraftJsonEntity GetInheritsMinecraftJsonEntity(string name) =>
            GetMinecraftJsonEntity(GetMinecraftJsonEntity(name).inheritsFrom);

        /// <summary>
        /// 解析所有versions目录下Minecraft的json文件
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<MinecraftJsonEntity> GetMinecraftJsonEntities() => GetMinecraftFileEntities()
            .Select(x => GetMinecraftJsonEntity(x.Root.GetFileName()));

        /// <summary>
        /// 获取指定Minecraft版本的对应assetsIndex，如1.12.2 => 1.12 1.7.2 => legacy
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetMinecraftVersionRoot(string name)
        {
            try
            {
                return Version.Parse(GetMinecraftJsonEntity(name).assets).ToString();
            }
            //假如没有assets这个属性或者assets属性不是一个有效的版本
            catch
            {
                if (GetMinecraftJsonEntity(name).assets == "legacy")
                {
                    return GetMinecraftJsonEntity(name).assets;
                }
                //假如此对象是加载器类型，尝试获取其继承自的minecraft的assets
                try
                {
                    return Version.Parse(GetInheritsMinecraftJsonEntity(name).assets).ToString();
                }
                //假如它根本没有inheritsFrom这个属性
                catch
                {
                    try
                    {
                        return GetMinecraftVersionRoot(name.ReplaceToVersion());
                    }
                    catch
                    {
                        var sp = name.ReplaceToVersion().Split('.');
                        try
                        {
                            var ver = Version.Parse($"{sp[0]}.{sp[1]}");

                            return ver < Version.Parse("1.7.10") ? "legacy" : ver.ToString();
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"解析失败{e.Message}");
                        }
                    }
                    
                }
            }
        }
        
        /// <summary>
        /// 获取Minecraft版本的json实体的类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public MinecraftJsonType GetMinecraftJsonType(string name)
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