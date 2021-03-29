using System.Linq;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Downloaders.Minecraft;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Network;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Downloaders
{
    public class MinecraftDownloader : Downloader
    {
        public MinecraftLocator Locator { get; set; }

        public MinecraftDownloader(MinecraftLocator locator = null)
        {
            Locator = locator;
        }

        private async Task<JObject> FetchManifest()
        {
            var manifest = await HttpUtils.Get("https://launchermeta.mojang.com/mc/game/version_manifest.json");

            return JObject.Parse(manifest.Content);
        }

        internal async Task<Minecraft> GetMinecraft(string id)
        {
            var versions = (await FetchManifest())["versions"].ToObject<JArray>();
            var version = versions.First(token => token["id"].ToString() == id);

            var mc = JObject.Parse((await HttpUtils.Get(version["url"].ToString())).Content);

            return Locator.GetMinecraft(mc);
        }

        public async Task<LatestMinecrafts> GetLatestMinecrafts()
        {
            var latest = (await FetchManifest())["latest"];

            return new LatestMinecrafts
            {
                Release = latest["release"].ToString(),
                Snapshot = latest["snapshot"].ToString()
            };
        }
    }
}