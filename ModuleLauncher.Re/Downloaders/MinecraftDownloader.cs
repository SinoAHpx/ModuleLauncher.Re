using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Downloaders;
using ModuleLauncher.Re.Models.Downloaders.Minecraft;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Downloaders
{
    public class MinecraftDownloader : DownloaderBase
    {
        #region Constructor, Fields, Properties

        protected override List<(string, FileInfo)> Files { get; set; }

        public DownloaderSource Source { get; set; } = DownloaderSource.Mojang;

        private const string ManifestUrl = "http://launchermeta.mojang.com/mc/game/version_manifest.json";

        #endregion

        #region Private helpers

        /// <summary>
        /// Get manifest.json from defined constant manifest url
        /// </summary>
        /// <returns></returns>
        private async Task<string> FetchManifest()
        {
            var response = await HttpUtility.Get(ManifestUrl);
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        #endregion
        
        /// <summary>
        /// Get latest minecrafts
        /// </summary>
        /// <returns>Item1 is release, Item2 is snapshot</returns>
        public async Task<(string, string)> GetLatestVersions()
        {
            var manifest = (await FetchManifest()).ToJObject();
            var re = (manifest.Fetch("latest.release"), manifest.Fetch("latest.snapshot"));

            return re;
        }

        /// <summary>
        /// Get remote minecrafts collections
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MinecraftDownloadItem>> GetRemoteMinecrafts()
        {
            var manifest = await FetchManifest();
            var versions = manifest.Fetch("versions").ToObject<JArray>();
            var re = new List<MinecraftDownloadItem>();
            
            foreach (var token in versions!)
            {
                var item = JsonConvert.DeserializeObject<MinecraftDownloadItem>(token.ToString());

                re.Add(item);
            }
            
            return re;
        }

        /// <summary>
        /// Get single minecraft via minecraft id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MinecraftDownloadItem> GetRemoteMinecraft(string id)
        {
            var minecrafts = await GetRemoteMinecrafts();

            return minecrafts.First(x => x.Id == id);
        }
    }
}