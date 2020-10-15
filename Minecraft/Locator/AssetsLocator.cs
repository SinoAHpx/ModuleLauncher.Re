using System;
using System.Collections.Generic;
using System.IO;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;
using ModuleLauncher.Re.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    //head
    public partial class AssetsLocator
    {
        private string _downloadLink;
        private MinecraftDownloadSource _downloadSource;

        public AssetsLocator(MinecraftLocator locator = null,
            MinecraftDownloadSource downloadSource = MinecraftDownloadSource.Bmclapi)
        {
            Locator = locator;
            DownloadSource = downloadSource;
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

            foreach (var x in jObj) yield return GetAsset(x.Value.GetValue("hash"));
        }

        public MinecraftAssetsEntity GetAssetsIndex(string name)
        {
            var root = Locator.GetMinecraftVersionRoot(name);

            return root == "legacy"
                ? new MinecraftAssetsEntity
                {
                    Name = "legacy.json",
                    Path = $"{Locator.Location}\\assets\\indexes\\legacy.json",
                    Link = $"{_downloadLink}/v1/packages/770572e819335b6c0a053f8378ad88eda189fc14/legacy.json"
                }
                : GetIndexEntity(name);
        }
    }

    //inside
    public partial class AssetsLocator
    {
        private MinecraftAssetsEntity GetAsset(string hash)
        {
            var secHash = hash.Substring(0, 2);
            return new MinecraftAssetsEntity
            {
                Name = hash,
                Path = $"{Locator.Location}\\assets\\objects\\{secHash}\\{hash}",
                Link = $"{_downloadLink}/{secHash}/{hash}"
            };
        }

        private MinecraftAssetsEntity GetIndexEntity(string name)
        {
            var type = Locator.GetMinecraftJsonType(name);
            var entity = type == MinecraftJsonType.Loader || type == MinecraftJsonType.LoaderNew
                ? Locator.GetInheritsMinecraftJsonEntity(name)
                : Locator.GetMinecraftJsonEntity(name);

            var index = entity.assetIndex;
            return index != null
                ? new MinecraftAssetsEntity
                {
                    Name = $"{index["id"]}.json",
                    Path = $"{Locator.Location}\\assets\\indexes\\{index["id"]}.json",
                    Link = $"{_downloadLink}/v1/packages/{index["sha1"]}/{index["id"]}.json"
                }
                : new MinecraftAssetsEntity {Link = null, Name = null, Path = null};
        }
    }
}