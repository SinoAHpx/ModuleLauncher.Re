using System;
using System.Collections.Generic;
using System.Linq;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;
using ModuleLauncher.Re.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    //head
    public partial class LibrariesLocator
    {
        private string _downloadLink;
        private MinecraftDownloadSource _downloadSource;

        public LibrariesLocator(MinecraftLocator location = null,
            MinecraftDownloadSource source = MinecraftDownloadSource.Bmclapi)
        {
            Locator = location;
            DownloadSource = source;
        }

        public MinecraftLocator Locator { get; set; }

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
                    case MinecraftDownloadSource.Bmclapi:
                        _downloadLink = "https://bmclapi2.bangbang93.com/maven";
                        break;
                    case MinecraftDownloadSource.Mcbbs:
                        _downloadLink = "https://download.mcbbs.net/maven";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
    }

    //exposed
    public partial class LibrariesLocator
    {
        /// <summary>
        ///     获取指定Minecraft的所有libraries
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

            re.AddRange(GetLibraryNames(name).Select(x => new MinecraftLibrariesEntity
            {
                Name = x.GetFileName(),
                Path = $"{Locator.Location}\\libraries\\{x}",
                Link = $"{link}/{x.Replace('\\', '/')}",
                UnformattedName = x.ToSrcFormat()
            }));

            return re.DistinctBy(x => x.Link);
        }

        public IEnumerable<MinecraftLibrariesEntity> GetNatives(string name)
        {
            var re = new List<MinecraftLibrariesEntity>();
            var type = Locator.GetMinecraftJsonType(name);
            if (type == MinecraftJsonType.Loader || type == MinecraftJsonType.LoaderNew)
            {
                re.AddRange(GetNatives(Locator.GetInheritsMinecraftJsonEntity(name).id));
            }

            re.AddRange(GetNativeNames(name).Select(x => new MinecraftLibrariesEntity
            {
                Name = x.GetFileName(),
                Link = $"{_downloadLink}/{x.Replace('\\', '/')}",
                Path = $"{Locator.Location}\\libraries\\{x}",
                UnformattedName = x.ToSrcFormat()
            }));

            return re.DistinctBy(x => x.Link);
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
            try
            {
                var entity = Locator.GetMinecraftJsonEntity(name);
                
                return entity.libraries.Where(IsLibAllow)
                    .Select(x => x["name"]?.ToString().ToLibFormat());
            }
            catch (Exception e)
            {
                throw new Exception($"获取minecraft实体失败\n{e.Message}");
            }
        }

        /// <summary>
        /// 判断此对象是否应该被添加
        /// </summary>
        /// <param name="s">libraries子对象</param>
        /// <returns></returns>
        private bool IsLibAllow(JToken s)
        {
            if (!s.IsPropertyExist("rules")) return true;
            
            var rules = JArray.Parse(s["rules"]?.ToString() ?? throw new Exception("json文件损坏"));
            foreach (var x in rules)
            {
                if (x.IsPropertyExist("os"))
                {
                    if (x.GetValue("action") == "allow")
                        return x["os"].GetValue("name") == "windows";
                    
                    if (x.GetValue("action") != "disallow") continue;
                    if (x["os"].GetValue("name") == "windows")
                        return false;
                }
                else
                {
                    return true;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 获取指定Minecraft版本json中natives的相对路径
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private IEnumerable<string> GetNativeNames(string name)
        {
            var re = new List<string>();

            Locator.GetMinecraftJsonEntity(name).libraries.Where(IsLibAllow).ForEach(x =>
            {
                try
                {
                    // ReSharper disable once PossibleNullReferenceException
                    var classifier = JObject.Parse(JsonConvert.SerializeObject(x["downloads"]["classifiers"]));
                    if (classifier.TryGetValue("natives-windows", out var n1))
                        re.Add(n1?["url"]?.ConvertUrl2Native());

                    if (classifier.TryGetValue("natives-windows-32", out var n2))
                        re.Add(n2?["url"]?.ConvertUrl2Native());

                    if (classifier.TryGetValue("natives-windows-64", out var n3))
                        re.Add(n3?["url"]?.ConvertUrl2Native());
                }
                catch
                {
                    if (x.IncludeStr("natives-windows"))
                        try
                        {
                            var addition = x["natives"].GetValue("windows", throwEx: true);
                            re.Add($"{x["name"].ToLibFormat(true)}-{addition}.jar");
                            re = re.Where(z => IsLibAllow(z)).ToList();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception($"json文件损坏:{exception.Message}");
                        }
                }
            });

            return re;
        }
    }
}