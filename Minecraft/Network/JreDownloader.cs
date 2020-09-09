using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Minecraft.Network;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Minecraft.Network
{
    //async
    public partial class JreDownloader
    {
        public static async Task<IEnumerable<JreDownloaderEntity>> GetJresAsync()
        {
            var array = JArray.Parse((await HttpHelper.GetHttpAsync("https://download.mcbbs.net/java/list")).Content);
            var re = new List<JreDownloaderEntity>();
            
            array.ForEach(x =>
            {
                re.Add(new JreDownloaderEntity
                {
                    Name = x.GetValue("file"),
                    Url = $"https://bmclapi.bangbang93.com/java/{x["file"]}"
                });
            });

            return re;
        }
        
        public static async Task<JreDownloaderEntity> GetJreAsync(bool osBit = true)
        {
            var ls = await GetJresAsync();
            var key = osBit ? "64.exe" : "32.exe";
            var re = new JreDownloaderEntity();
            ls.ForEach(x =>
            {
                if (x.Name.Contains(key)) re = x;
            });

            return re;
        }
    }
    
    //sync
    public partial class JreDownloader
    {
        public static IEnumerable<JreDownloaderEntity> GetJres() => GetJresAsync().GetResult();
        
        public static JreDownloaderEntity GetJre(bool osBit = true) => GetJreAsync(osBit).GetResult();
    }
}