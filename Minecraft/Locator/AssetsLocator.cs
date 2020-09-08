using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;
using ModuleLauncher.Re.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    //head
    public partial class AssetsLocator
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
                        _downloadLink = "http://resources.download.minecraft.net";
                        break;
                    case MinecraftDownloadSource.Bmclapi:
                        _downloadLink = "https://bmclapi2.bangbang93.com/assets";
                        break;
                    case MinecraftDownloadSource.Mcbbs:
                        _downloadLink = "https://download.mcbbs.net/assets";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public AssetsLocator(MinecraftLocator locator = null, MinecraftDownloadSource downloadSource = MinecraftDownloadSource.Bmclapi)
        {
            Locator = locator;
            DownloadSource = downloadSource;
        }
    }
    
    //exposed
    public partial class AssetsLocator
    {
        public IEnumerable<MinecraftAssetsEntity> GetAssets(string name)
        {
            var jText = File.ReadAllText(
                $"{Locator.Location}\\assets\\indexes\\{Locator.GetMinecraftVersionRoot(name)}.json");
            var jObj = JObject.Parse(jText)["objects"]?.ToObject<JObject>();
            
            if (jObj == null) yield break;
            
            foreach (var x in jObj)
            {
                var token = x.Value;
                
                var hash = token["hash"]?.ToString();
                if (hash == null) continue;
                
                var secHash = hash.Substring(0, 2);
                yield return new MinecraftAssetsEntity
                {
                    Name = hash,
                    Path = $"{Locator.Location}\\assets\\objects\\{secHash}\\{hash}",
                    Link = $"{_downloadLink}/{secHash}/{hash}"
                };
            }
        }
    }
    
    //inside
    public partial class AssetsLocator
    {
        
    }
}