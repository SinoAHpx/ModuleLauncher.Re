using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Downloaders;
using AHpx.ModuleLauncher.Utils.Network;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Downloaders.Locator
{
    public class MinecraftDownloader : DownloaderCore
    {
        public MinecraftDownloader() : base(){}

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

        public async Task<JObject> GetMinecraftItem(string version)
        {
            var arr = JObject.Parse((await HttpUtils.Get(_manifest)).Content)["versions"].ToObject<JArray>();

            return arr.FirstOrDefault(w => w["id"].ToString() == version).ToObject<JObject>();
        }
    }
}