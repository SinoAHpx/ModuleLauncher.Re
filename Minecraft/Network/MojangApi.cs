using System.Collections.Generic;
using System.Threading.Tasks;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Minecraft.Network;
using ModuleLauncher.Re.Utils;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Minecraft.Network
{
    public partial class MojangApi
    {
        public static async Task<IEnumerable<MojangServiceStatus>> GetMojangServiceStatusesAsync()
        {
            const string api = "https://status.mojang.com/check";
            var response = await HttpHelper.GetHttpAsync(api);
            var array = JArray.Parse(response.Content);
            var re = new List<MojangServiceStatus>();

            foreach (var jToken in array)
            {
                var o = (JObject) jToken;
                o.ForEach((s, token) =>
                {
                    re.Add(new MojangServiceStatus
                    {
                        Server = s,
                        Status = token.ToString()
                    });
                });
            }

            return re;
        }
    }

    public partial class MojangApi
    {
    }
}