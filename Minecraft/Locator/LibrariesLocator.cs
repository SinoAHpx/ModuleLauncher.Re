using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpCompress.Archives.SevenZip;

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
            
            re.AddRange(CollectionHelper.ExcludeRepeat(GetLibraryNames(name).Select(x => new MinecraftLibrariesEntity
            {
                Name = x.GetFileName(),
                Path = $"{Locator.Location}\\libraries\\{x}",
                Link = $"{link}/{x.Replace('\\', '/')}",
                UnformattedName = x.ToSrcFormat()
            }).DistinctBy(x => x.Link)));
            
            return re;
        }

        public IEnumerable<MinecraftLibrariesEntity> GetNatives(string name)
        {
            //return GetNativeNames(name);
            var libraries = GetLibraries(name);
            var re = new List<MinecraftLibrariesEntity>();
            
            foreach (var entity in libraries)
            {
                if (!File.Exists(entity.Path)) continue;

                var zip = new ZipArchive(File.OpenRead(entity.Path));
                
                foreach (var entry in zip.Entries)
                {
                    if (entry.FullName.Split('/').Length > 1)
                    {
                        continue;
                    }
                    Console.WriteLine(entry.FullName);
                    re.Add(entity);
                    /*if (entry.FullName.EndsWith("dll"))
                    {
                       
                        break;
                    }*/
                }
            }

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
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private IEnumerable<string> GetLibraryNames(string name)
        {
            return null;
        }

        public IEnumerable<string> GetNativeNames(string name)
        {
            var libraries = Locator.GetMinecraftJsonEntity(name).libraries;
            var re = new List<string>();
            
            libraries.ForEach(x =>
            {
                try
                {
                    var classifier = JObject.Parse(JsonConvert.SerializeObject(x["downloads"]["classifiers"]));
                    if (classifier.TryGetValue("natives-windows",out var n1)) 
                        re.Add(n1?["url"]?.ConvertUrl2Native());
                    
                    if (classifier.TryGetValue("natives-windows-32",out var n2))
                        re.Add(n2?["url"]?.ConvertUrl2Native());
                    
                    if (classifier.TryGetValue("natives-windows-64",out var n3))
                        re.Add(n3?["url"]?.ConvertUrl2Native());

                }    
                catch (Exception e)
                {
                    if (x.IncludeStr("natives-windows"))
                    {
                        try
                        {
                            var addition = x["natives"].GetValue("windows", throwEx: true);
                            re.Add($"{x["name"].ToLibFormat(true)}-{addition}.jar");
                        }
                        catch (Exception exception)
                        {
                            throw new Exception($"json文件损坏:{exception.Message}");
                        }
                    }
                }
            });

            return re;
        }
    }
}