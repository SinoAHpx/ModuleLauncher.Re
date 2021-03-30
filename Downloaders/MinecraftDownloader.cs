using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Downloaders;
using AHpx.ModuleLauncher.Data.Downloaders.Minecraft;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using AHpx.ModuleLauncher.Utils.Network;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Downloaders
{
    public class MinecraftDownloader : Downloader
    {
        public MinecraftLocator Locator { get; set; }
        public MinecraftDownloadSource DownloadSource { get; set; }

        public MinecraftDownloader(MinecraftLocator locator = null, MinecraftDownloadSource source = MinecraftDownloadSource.Official)
        {
            Locator = locator;
            DownloadSource = source;
        }

        private async Task<JObject> FetchManifest()
        {
            var manifest = await HttpUtils.Get("https://launchermeta.mojang.com/mc/game/version_manifest.json");

            return JObject.Parse(manifest.Content);
        }
        
        public async Task<MinecraftItem> GetMinecraft(string id, bool fetchJson = false)
        {
            var versions = (await FetchManifest())["versions"].ToObject<JArray>();
            var version = versions.First(token => token["id"].ToString() == id);

            var mc = JObject.Parse((await HttpUtils.Get(version["url"].ToString())).Content);

            return new MinecraftItem
            {
                Minecraft = Locator.GetMinecraft(mc),
                Id = version["id"].ToString(),
                Type = version["type"].ToString(),
                ReleaseTime = version["releaseTime"].ToString()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fetchJson">建议保持false，否则容易卡死。如果为false，则Minecraft属性为null</param>
        /// <returns></returns>
        public async Task<IEnumerable<MinecraftItem>> GetMinecrafts(bool fetchJson = false)
        {
            var versions = (await FetchManifest())["versions"].ToObject<JArray>();
            var re = new List<MinecraftItem>();
            
            foreach (var token in versions)
            {
                Console.WriteLine(token["id"].ToString() + " is running!");
                re.Add(new MinecraftItem
                {
                    Minecraft = fetchJson
                        ? Locator.GetMinecraft(JObject.Parse((await HttpUtils.Get(token["url"].ToString())).Content))
                        : null,
                    Id = token["id"].ToString(),
                    Type = token["type"].ToString(),
                    ReleaseTime = token["releaseTime"].ToString()
                });
            }

            return re;
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

        private string FetchMinecraftDownloadLink(Minecraft minecraft, out string sha1)
        {
            var s = minecraft.Json.Downloads["client"]["sha1"].ToString();
            var re = DownloadSource switch
            {
                MinecraftDownloadSource.Official => minecraft.Json.Downloads["client"]["url"].ToString(),
                MinecraftDownloadSource.BmclApi =>
                    $"https://bmclapi2.bangbang93.com/mc/game/{minecraft.Json.Id}/client/{s}/client.jar",
                MinecraftDownloadSource.Mcbbs =>
                    $"https://download.mcbbs.net/mc/game/{minecraft.Json.Id}/client/{s}/client.jar",
                _ => throw new ArgumentOutOfRangeException()
            };

            sha1 = s;
            return re;
        }
        
        public async Task Download(string id)
        {
            var mc = (await GetMinecraft(id, true)).Minecraft;
            
            if (!mc.File.Version.Exists)
                mc.File.Version.Create();

            await File.WriteAllTextAsync(mc.File.Json.FullName, mc.OriginalJson.ToString());
            
            try
            {
                var address = FetchMinecraftDownloadLink(mc, out var sha1);

                if (mc.File.Jar.Exists)
                {
                    if (mc.File.Jar.GetSha1() != sha1)
                    {
                        mc.File.Jar.Delete();
                        
                        await base.Download(new DownloadItem
                        {
                            Address = address,
                            FileName = mc.File.Jar.FullName
                        });
                    }
                }
                
                await base.Download(new DownloadItem
                {
                    Address = address,
                    FileName = mc.File.Jar.FullName
                });
            }
            catch
            {
                Console.WriteLine("Download failed by using other source");
                DownloadSource = MinecraftDownloadSource.Official;
                try
                {
                    await Download(id);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private string FetchLibrariesDownloadLink(Library lib)
        {
            var re = DownloadSource switch
            {
                MinecraftDownloadSource.Official => $"https://libraries.minecraft.net/{lib.RelativeUrl}",
                MinecraftDownloadSource.BmclApi =>
                    $"https://bmclapi2.bangbang93.com/maven/{lib.RelativeUrl}",
                MinecraftDownloadSource.Mcbbs =>
                    $"https://download.mcbbs.net/maven/{lib.RelativeUrl}",
                _ => throw new ArgumentOutOfRangeException()
            };
            
            return re;
        }
        
        public async Task DownloadLibraries(string id)
        {
            var dList = new List<DownloadItem>();
            var mc = (await GetMinecraft(id, true)).Minecraft;

            foreach (var library in Locator.GetLibraries(mc, false))
            {
                dList.Add(new DownloadItem
                {
                    Address = FetchLibrariesDownloadLink(library),
                    FileName = library.File.FullName
                });
            }

            await base.Download(dList, 5);
        }
    }
}