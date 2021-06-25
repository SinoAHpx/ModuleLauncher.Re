using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Downloaders;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;

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
    }
}