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
    public partial class OptifineDownloader
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
    //async
    public partial class OptifineDownloader
    {
        public static async Task<List<OptifineDownloaderEntity>> GetOptifinesAsync(string id)
        {
            var array = JArray.Parse((await HttpHelper.GetHttpAsync($"https://download.mcbbs.net/optifine/{id}"))
                .Content).ToList();

            var re = new List<OptifineDownloaderEntity>();
            array.ForEach(x =>
            {
                re.Add(new OptifineDownloaderEntity
                {
                    Name = x.GetValue("filename"),
                    Url = $"{_downloadLink}/optifine/{x["mcversion"]}/{x["type"]}/{x["patch"]}",
                    McVersion = x.GetValue("mcversion"),
                    Patch = x.GetValue("patch"),
                    Type = x.GetValue("type"),
                    FileName = $"OptiFine_{x["mcversion"]}_{x["type"]}_{x["patch"]}.jar"
                });
            });

            return re;
        }        
    }
    
    //sync
    public partial class OptifineDownloader
    {
        public static List<OptifineDownloaderEntity> GetOptifines(string id) => GetOptifinesAsync(id).GetResult();
    }
}