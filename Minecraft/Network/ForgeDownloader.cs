using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Minecraft.Network;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Minecraft.Network
{
    //head
    public partial class ForgeDownloader
    {
        private static string _downloadLink = "https://bmclapi2.bangbang93.com";
        public static MinecraftDownloadSource DownloadSource
        {
            set
            {
                switch (value)
                {
                    case MinecraftDownloadSource.Mojang:
                    case MinecraftDownloadSource.Mcbbs:
                        _downloadLink = "https://bmclapi2.bangbang93.com";
                        break;
                    case MinecraftDownloadSource.Bmclapi:
                        _downloadLink = "https://download.mcbbs.net";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
    }
    
    public partial class ForgeDownloader
    {
        public static async Task<List<MinecraftForgeDownloaderEntity>> GetForgesAsync(string id)
        {
            var array = JArray.Parse(
                (await HttpHelper.GetHttpAsync($"https://download.mcbbs.net/forge/minecraft/{id}")).Content).ToList();

            var re = new List<MinecraftForgeDownloaderEntity>();
            array.ForEach(x =>
            {
                re.Add(new MinecraftForgeDownloaderEntity
                {
                    Build = x.GetValue("build"),
                    McVersion = x.GetValue("mcversion"),
                    Version = x.GetValue("version"),
                    Url = $"{_downloadLink}/forge/download/{x["build"]}",
                    FileName = $"forge-{x.GetValue("mcversion")}-{x.GetValue("version")}-installer.jar"
                });
            });

            return re;
        }

        public static List<MinecraftForgeDownloaderEntity> GetForge(string id) => GetForgesAsync(id).GetResult();
    }
}