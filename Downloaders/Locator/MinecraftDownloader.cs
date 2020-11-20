using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Downloaders;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using AHpx.ModuleLauncher.Utils.Network;
using Newtonsoft.Json;
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

        public async Task<IEnumerable<MinecraftItem>> GetMinecrafts()
        {
            var arr = JObject.Parse((await HttpUtils.Get(_manifest)).Content)["versions"].ToObject<JArray>();
            var re = new List<MinecraftItem>();
            
            arr.ForEach(x =>
            {
                re.Add(JsonConvert.DeserializeObject<MinecraftItem>(x.ToString()));
            });

            return re;
        }
        
        public async Task<string> GetMinecraftJson(string version)
        {
            var arr = JObject.Parse((await HttpUtils.Get(_manifest)).Content)["versions"].ToObject<JArray>();
            var obj = arr.FirstOrDefault(w => w["id"].ToString() == version).ToObject<JObject>();

            return (await HttpUtils.Get(obj["url"].ToString())).Content;
        }

        public async Task Download(string version, MinecraftLocator locator, bool checkExist = true)
        {
            var mc = locator.GetMinecraft(JObject.Parse(await GetMinecraftJson(version)));
            
            if (locator is LibrariesLocator lib)
            {
                mc.File.Libraries.Create();
                
                foreach (var library in lib.GetLibraries(mc).Concat(lib.GetNatives(mc)))
                {
                    if (library.File.Exists && checkExist) continue;
                    
                    var relativeUrl = library.File.FullName.ToRelativeUrl("\\libraries\\");
                    var url = Source switch
                    {
                        DownloadSource.Official => $"https://libraries.minecraft.net/{relativeUrl}",
                        DownloadSource.BmclApi =>
                            $"https://bmclapidoc.bangbang93.com/maven/{relativeUrl}",
                        DownloadSource.Mcbbs => $"https://download.mcbbs.net/maven/{relativeUrl}",
                        _ => throw new IndexOutOfRangeException("No such source!")
                    };
                        
                    await base.Download(url, library.File);
                }
            }
            else if (locator is AssetsLocator ass)
            {
                mc.File.Assets.Create();
                if (!File.Exists($@"{mc.File.Assets}\indexes\{mc.RootVersion}.json"))
                {
                    var index = mc.Json.AssetIndex["url"].ToString();

                    await base.Download(index, new FileInfo($@"{mc.File.Assets}\indexes\{mc.RootVersion}.json"));
                }

                foreach (var asset in ass.GetAssets(mc))
                {
                    if (asset.File.Exists && checkExist) continue;
                    
                    var relativeUrl = asset.File.FullName.ToRelativeUrl("\\objects\\");
                    var url = Source switch
                    {
                        DownloadSource.Official => $"http://resources.download.minecraft.net/{relativeUrl}",
                        DownloadSource.BmclApi =>
                            $"https://bmclapi2.bangbang93.com/assets/{relativeUrl}",
                        DownloadSource.Mcbbs => $"https://download.mcbbs.net/assets/{relativeUrl}",
                        _ => throw new IndexOutOfRangeException("No such source!")
                    };
                        
                    await base.Download(url, asset.File);
                }
            }
            else
            {
                mc.File.Version.Create();
                
                var obj = JObject.Parse(await GetMinecraftJson(version));
                var sha1 = obj["downloads"]["client"]["sha1"].ToString();
                var url = Source switch
                {
                    DownloadSource.Official => $"https://launcher.mojang.com/v1/objects/{sha1}/client.jar",
                    DownloadSource.BmclApi =>
                        $"https://bmclapidoc.bangbang93.com/mc/game/{version}/client/{sha1}/client.jar",
                    DownloadSource.Mcbbs => $"https://download.mcbbs.net/mc/game/{version}/client/{sha1}/client.jar",
                    _ => throw new IndexOutOfRangeException("No such source!")
                };

                if (checkExist)
                {
                    if (!mc.File.Json.Exists)
                        await File.WriteAllTextAsync(mc.File.Json.FullName, obj.ToString());
                }
                else
                {
                    await File.WriteAllTextAsync(mc.File.Json.FullName, obj.ToString());
                }

                if (checkExist)
                {
                    if (!mc.File.Jar.Exists)
                        await base.Download(url, new FileInfo(mc.File.Jar.FullName));
                }
                else
                {
                    await base.Download(url, new FileInfo(mc.File.Jar.FullName));
                }
            }
        }
    }
}