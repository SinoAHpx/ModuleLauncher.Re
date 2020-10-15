using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Minecraft.Network;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Minecraft.Network
{
    public partial class MojangApi
    {
        /// <summary>
        ///     返回各类Mojang服务的状态。
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     这将返回在提供的时间戳时该用户名的UUID。
        /// </summary>
        /// <param name="name">是该uuid的当前名称，而并不是请求的名称</param>
        /// <param name="timestamp">时间戳是指UNIX时间戳（不包含毫秒），当此参数为空时，将使用当前时间</param>
        /// <returns></returns>
        public static async Task<MojangUUIDProfile> GetUuidAsync(string name, string timestamp = null)
        {
            var api = timestamp.IsNullOrEmpty()
                ? $"https://api.mojang.com/users/profiles/minecraft/{name}"
                : $"https://api.mojang.com/users/profiles/minecraft/{name}?at={timestamp}";

            var response = await HttpHelper.GetHttpAsync(api);
            var obj = JObject.Parse(response.Content);

            switch (response.StatusCode)
            {
                case HttpStatusCode.NoContent:
                    throw new ArgumentException("指定用户不存在");
                case HttpStatusCode.BadRequest:
                    throw new ArgumentOutOfRangeException(nameof(timestamp), "时间戳超出范围");
            }

            return new MojangUUIDProfile
            {
                Name = obj["name"]?.ToString(),
                Uuid = obj["id"]?.ToString()
            };
        }

        public static async Task<IEnumerable<MojangHistoryName>> GetHistoryNamesAsync(string uuid)
        {
            var api = $"https://api.mojang.com/user/profiles/{uuid}/names";
            var response = await HttpHelper.GetHttpAsync(api);
            var array = JArray.Parse(response.Content);

            return array.Select(token => new MojangHistoryName
            {
                Name = token.GetValue("name"),
                ChangedAt = token.GetValue("changedToAt")
            }).ToList();
        }
    }

    public partial class MojangApi
    {
    }
}