using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    //head
    public partial class LibrariesLocator
    {
        public MinecraftLocator Locator { get; set; }

        private string _downloadLink;
        private MinecraftDownloadSource _downloadSource;
        
        public MinecraftDownloadSource DownloadSource
        {
            get => _downloadSource;
            set
            {
                _downloadSource = value;
                switch (value)
                {
                    case MinecraftDownloadSource.Mojang:
                        _downloadLink = "https://libraries.minecraft.net";
                        break;
                    case MinecraftDownloadSource.Mcbbs:
                        _downloadLink = "https://bmclapi2.bangbang93.com/maven";
                        break;
                    case MinecraftDownloadSource.Bmclapi:
                        _downloadLink = "https://download.mcbbs.net/maven";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                } 
            }
        }
        
        public LibrariesLocator(MinecraftLocator location = null, MinecraftDownloadSource source = MinecraftDownloadSource.Bmclapi)
        {
            Locator = location;
            DownloadSource = source;
        }
    }
    
    //exposed
    public partial class LibrariesLocator
    {
        /// <summary>
        /// 获取指定Minecraft的所有libraries
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<MinecraftLibrariesEntity> GetLibraries(string name)
        {
            var type = Locator.GetMinecraftJsonType(name);
            var re = new List<MinecraftLibrariesEntity>();
            var link = _downloadLink;
            
            if (type == MinecraftJsonType.Loader || type == MinecraftJsonType.LoaderNew)
            {
                re.AddRange(GetLibraries(Locator.GetInheritsMinecraftJsonEntity(name).id));
                if (DownloadSource == MinecraftDownloadSource.Mojang)
                    link = "https://bmclapi2.bangbang93.com/maven";
            }
            
            re.AddRange(CollectionHelper.RemoveRepeat(GetLibraryNames(name, true).Select(x => new MinecraftLibrariesEntity
            {
                Name = x.GetFileName(),
                Path = $"{Locator.Location}\\libraries\\{x}",
                Link = $"{link}/{x.Replace('\\', '/')}",
                UnformattedName = x.ToSrcFormat()
            }).DistinctBy(x => x.Link)));
            
            return re;
        }
    }
    
    //inside
    public partial class LibrariesLocator
    {
        /// <summary>
        /// 获取指定Minecraft版本json中libraries的name值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="format">把它整成路径或者url的样子</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private IEnumerable<string> GetLibraryNames(string name, bool format = false)
        {
            try
            {
                var entity = Locator.GetMinecraftJsonEntity(name);
                return entity.libraries.Select(x =>
                    format ? x["name"]?.ToString().ToLibFormat() : x["name"]?.ToString());
            }
            catch (Exception e)
            {
                throw new Exception($"获取minecraft实体失败\n{e.Message}");
            }
        }
    }
}