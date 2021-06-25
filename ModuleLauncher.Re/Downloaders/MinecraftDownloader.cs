using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.Locators;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Downloaders;
using ModuleLauncher.Re.Models.Downloaders.Minecraft;
using ModuleLauncher.Re.Models.Locators.Minecraft;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Downloaders
{
    public class MinecraftDownloader : DownloaderBase
    {
        #region Constructor, Fields, Properties

        private readonly LocalityLocator _locator;

        protected override List<(string, FileInfo)> Files { get; set; } = new List<(string, FileInfo)>();

        public DownloaderSource Source { get; set; } = DownloaderSource.Mojang;

        private const string ManifestUrl = "http://launchermeta.mojang.com/mc/game/version_manifest.json";

        public MinecraftDownloader(LocalityLocator locator)
        {
            _locator = locator;
        }

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

        /// <summary>
        /// Fetch minecraft json via specify minecraft id
        /// </summary>
        /// <returns></returns>
        private async Task<JObject> FetchMinecraftJson(string id)
        {
            var mc = await GetRemoteMinecraft(id);
            var response = await HttpUtility.Get(mc.Url);

            var re = await response.Content.ReadAsStringAsync();

            return re.ToJObject();
        }

        private string GetDownloadUrl(JToken raw)
        {
            return Source switch
            {
                DownloaderSource.Mojang => raw.Fetch("downloads.client.url"),
                DownloaderSource.Bmclapi => $"https://bmclapi2.bangbang93.com/version/{raw.Fetch("id")}/client",
                DownloaderSource.Mcbbs => $"https://download.mcbbs.net/version/{raw.Fetch("id")}/client",
                _ => throw new ArgumentOutOfRangeException()
            };
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

        /// <summary>
        /// Download minecraft jar & json via specify id
        /// </summary>
        /// <param name="id"></param>
        public async Task DownloadMinecraft(string id)
        {
            var json = await FetchMinecraftJson(id);
            var mc = _locator.GetLocalVersion(id, true);
            var url = GetDownloadUrl(json);

            //Write json file if it doesn't exist
            if (!mc.Json.Exists)
            {
                if (!mc.Version.Exists)
                {
                    mc.Version.Create();
                }
                
                await mc.Json.WriteAllText(json.ToString());
            }
            
            //Start to download jar file

            if (!mc.Jar.Exists)
            {
                Files.Add((url, mc.Jar));
            
                await base.Download();
            }
        }
    }
}