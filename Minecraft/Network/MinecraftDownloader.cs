using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Minecraft.Network;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Minecraft.Network
{
    //head
    public partial class MinecraftDownloader
    {
        private static readonly MinecraftDownloadLinkEntity DownloadLink = new MinecraftDownloadLinkEntity();

        public static MinecraftDownloadSource DownloadSource
        {
            set
            {
                switch (value)
                {
                    case MinecraftDownloadSource.Mojang:
                        DownloadLink.Jar = "https://launcher.mojang.com";
                        DownloadLink.Json = "https://launchermeta.mojang.com";
                        break;
                    case MinecraftDownloadSource.Mcbbs:
                        DownloadLink.Jar = DownloadLink.Json = "https://bmclapi2.bangbang93.com";
                        break;
                    case MinecraftDownloadSource.Bmclapi:
                        DownloadLink.Jar = DownloadLink.Json = "https://download.mcbbs.net";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
    }
    
    //async
    public partial class MinecraftDownloader
    {
        /// <summary>
        /// 获取最新的Minecraft
        /// </summary>
        /// <returns></returns>
        public static async Task<MinecraftDownloaderLatest> GetLatestMinecraftAsync()
        {
            var obj = await GetMainfestObjectAsync();
            return new MinecraftDownloaderLatest
            {
                Release = obj["latest"]?.GetValue("release"),
                Snapshot = obj["latest"]?.GetValue("snapshot")
            };
        }

        /// <summary>
        /// 获取所有Minecraft
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<MinecraftDownloaderItem>> GetMinecraftsAsync()
        {
            var array = JArray.Parse((await GetMainfestObjectAsync())["versions"]?.ToString() ??
                                     throw new Exception("Failed to parse main fest")).ToList();
            
            var re = new List<MinecraftDownloaderItem>();
            array.ForEach(x =>
            {
                re.Add(new MinecraftDownloaderItem
                {
                    Id = x.GetValue("id"),
                    Type = x.GetValue("type"),
                    Link = x.GetValue("url"),
                    ReleaseTime = x.GetValue("releaseTime")
                });
            });

            return re;
        }

        /// <summary>
        /// 获取指定类型的Minecraft
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<MinecraftDownloaderItem>> GetSpecifyMinecraftsAsync(
            MinecraftDownloaderType type)
        {
            var ls = await GetMinecraftsAsync();
            switch (type)
            {
                case MinecraftDownloaderType.Release:
                    return ls.Where(x => x.Type == "release").ToList();
                case MinecraftDownloaderType.Snapshot:
                    return ls.Where(x => x.Type == "snapshot").ToList();
                default:
                    return ls.Where(x => x.Type == "old_alpha" || x.Type == "old_beta").ToList();
            }
        }

        /// <summary>
        /// 获取指定的Minecraft
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<MinecraftDownloaderItem> GetMinecraftAsync(string id)
        {
            return (await GetMinecraftsAsync()).ToList().Find(x => x.Id == id);
        }
        
        /// <summary>
        /// 由指定的id获取指定的Minecraft下载链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<MinecraftDownloadLinkEntity> GetDownloadLinkAsync(string id)
        {
            var versions = (await GetMainfestObjectAsync())["versions"]?.ToObject<JArray>();
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once MethodHasAsyncOverload
            var token =  versions.ToList().Find(x => x.GetValue("id") == id);
            var entity = JObject.Parse((await HttpHelper.GetHttpAsync(token.GetValue("url"))).Content);

            var jar = entity["downloads"]?["client"].GetValue("url").Replace("https://launcher.mojang.com/","");
            var json = token.GetValue("url").Replace("https://launchermeta.mojang.com/","");
            
            return new MinecraftDownloadLinkEntity
            {
                Jar = $"{DownloadLink.Jar}/{jar}",
                Json = $"{DownloadLink.Json}/{json}"
            };
        }
    }

    //private
    public partial class MinecraftDownloader
    {
        private static async Task<JObject> GetMainfestObjectAsync()
        {
            try
            {
                return JObject.Parse(
                    (await HttpHelper.GetHttpAsync($"{DownloadLink.Json}/mc/game/version_manifest.json"))
                    .Content);
            }
            catch (Exception e)
            {
                throw new Exception($"mainfest.json解析失败\n{e.Message}");
            }
            
        }
    }

    //sync
    public partial class MinecraftDownloader
    {
        public static MinecraftDownloaderLatest GetLatestMinecraft() => GetLatestMinecraftAsync().GetResult();

        public static IEnumerable<MinecraftDownloaderItem> GetMinecrafts() => GetMinecraftsAsync().GetResult();

        public static IEnumerable<MinecraftDownloaderItem> GetSpecifyMinecrafts(MinecraftDownloaderType type) =>
            GetSpecifyMinecraftsAsync(type).GetResult();

        public static MinecraftDownloaderItem GetMinecraft(string id) => GetMinecraftAsync(id).GetResult();
        public static MinecraftDownloadLinkEntity GetDownloadLink(string id) => GetDownloadLinkAsync(id).GetResult();
    }
}