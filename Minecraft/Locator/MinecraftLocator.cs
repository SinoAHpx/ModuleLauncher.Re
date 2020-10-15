using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;
using ModuleLauncher.Re.Extensions;
using Newtonsoft.Json;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    //head
    public partial class MinecraftLocator
    {
        public MinecraftLocator(string location = ".\\.minecraft")
        {
            Location = location;
        }

        public string Location { get; set; }

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
        ///     获取versions目录下所有的Minecraft版本
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MinecraftFileEntity> GetMinecraftFileEntities()
        {
            return Directory
                .GetDirectories($"{Location}\\versions")
                .Select(x => GetMinecraftFileEntity(x.GetFileName()));
        }

        /// <summary>
        ///     获取指定的Minecraft版本
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
        ///     解析指定Minecraft版本的json文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public MinecraftJsonEntity GetMinecraftJsonEntity(string name)
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
        ///     解析loader类型客户端继承的MinecraftJson
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal MinecraftJsonEntity GetInheritsMinecraftJsonEntity(string name)
        {
            return GetMinecraftJsonEntity(GetMinecraftJsonEntity(name).inheritsFrom);
        }

        /// <summary>
        ///     解析所有versions目录下Minecraft的json文件
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<MinecraftJsonEntity> GetMinecraftJsonEntities()
        {
            return GetMinecraftFileEntities()
                .Select(x => GetMinecraftJsonEntity(x.Root.GetFileName()));
        }

        /// <summary>
        ///     获取指定Minecraft版本的对应assetsIndex，如1.12.2 => 1.12 1.7.2 => legacy
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
                if (GetMinecraftJsonEntity(name).assets == "legacy") return GetMinecraftJsonEntity(name).assets;
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
        ///     获取Minecraft版本的json实体的类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public MinecraftJsonType GetMinecraftJsonType(string name)
        {
            var entity = GetMinecraftJsonEntity(name);
            var verNew = Version.Parse("1.13");

            if (entity.type == "release")
                try
                {
                    var id = Version.Parse(entity.id);

                    //vanilla old
                    if (entity.assets == "legacy")
                        return MinecraftJsonType.VanillaOld;

                    //>=1.7.10 Vanilla
                    return id < verNew ? MinecraftJsonType.Vanilla : MinecraftJsonType.VanillaNew;
                }
                //loaders
                catch
                {
                    try
                    {
                        //>=1.7.10 Loader or 1.7.2 optifine
                        var inherit = GetInheritsMinecraftJsonEntity(name);

                        return Version.Parse(inherit.id) < verNew
                            ? MinecraftJsonType.Loader
                            : MinecraftJsonType.LoaderNew;
                    }
                    catch
                    {
                        try
                        {
                            var modify = Version.Parse(entity.assets);
                            return MinecraftJsonType.Modify;
                        }
                        catch
                        {
                            return MinecraftJsonType.LoaderOld;
                        }
                    }
                }


            return entity.assets == "legacy"
                ? MinecraftJsonType.VanillaOld
                : Version.Parse(entity.assets) < verNew
                    ? Version.Parse(entity.assets) < Version.Parse("1.7.10")
                        ? MinecraftJsonType.VanillaOld
                        : MinecraftJsonType.Vanilla
                    : MinecraftJsonType.VanillaNew;
        }
    }
}