using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Downloaders;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using AHpx.ModuleLauncher.Utils.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Downloaders.Locator
{
    public class MinecraftDownloader : DownloaderCore
    {
        private string _manifest = "http://launchermeta.mojang.com/mc/game/version_manifest.json";
        private DownloadSource _source;
        public DownloadSource Source
        {
            get => _source;
            set
            {
                _source = value;
                _manifest = value switch
                {
                    DownloadSource.Official => "http://launchermeta.mojang.com/mc/game/version_manifest.json",
                    DownloadSource.BmclApi => "https://bmclapi2.bangbang93.com/mc/game/version_manifest.json",
                    DownloadSource.Mcbbs => "https://download.mcbbs.net/mc/game/version_manifest.json",
                    _ => throw new IndexOutOfRangeException("No such source")
                };
            }
        }

        public async Task<string[]> GetLatestVersions()
        {
            var manifest = JObject.Parse((await HttpUtils.Get(_manifest)).Content);

            return new[] {manifest["latest"]["release"].ToString(), manifest["latest"]["snapshot"].ToString()};
        }

        public async Task<IEnumerable<MinecraftItem>> GetMinecraftItems()
        {
            var arr = JObject.Parse((await HttpUtils.Get(_manifest)).Content)["versions"].ToObject<JArray>();
            var re = new List<MinecraftItem>();
            
            arr.ForEach(x =>
            {
                re.Add(JsonConvert.DeserializeObject<MinecraftItem>(x.ToString()));
            });

            return re;
        }
        
        public async Task<MinecraftItem> GetMinecraftItem(string version)
        {
            var arr = await GetMinecraftItems();

            return arr.FirstOrDefault(x => x.Id == version);
        }
    }
}