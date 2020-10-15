using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Minecraft.Network;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;
using Newtonsoft.Json;
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

        /// <summary>
        ///     返回该用户之前使用过的所有用户名和当前使用的用户名。
        /// </summary>
        /// <param name="uuid">UUID必须不带连字符。</param>
        /// <returns></returns>
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

        /// <summary>
        ///     批量获取uuid，每个请求不能超过10个名字
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> GetUuidsByNamesAsync(IEnumerable<string> names)
        {
            var payload = JsonConvert.SerializeObject(names);
            var api = "https://api.mojang.com/profiles/minecraft";
            var response = await HttpHelper.PostHttpAsync(api, payload);
            var array = JArray.Parse(response.Content);
            var re = new List<string>();

            array.ForEach(x => re.Add(x.GetValue("id")));

            return re;
        }

        /// <summary>
        ///     返回已销售副本总数、最近24小时内售出的副本数量和每秒的销售量.
        /// </summary>
        /// <returns></returns>
        public static async Task<MojangStatistics> GetStatisticsAsync()
        {
            var payload = JsonConvert.SerializeObject(new
            {
                metricKeys = new[] {"item_sold_minecraft", "prepaid_card_redeemed_minecraft"}
            });

            var api = "https://api.mojang.com/orders/statistics";
            var response = await HttpHelper.PostHttpAsync(api, payload);

            return JsonConvert.DeserializeObject<MojangStatistics>(response.Content);
        }
    }

    public partial class MojangApi
    {
        public static IEnumerable<MojangServiceStatus> GetMojangServiceStatuses()
        {
            return GetMojangServiceStatusesAsync().GetResult();
        }

        public static MojangUUIDProfile GetUuid(string name, string timestamp)
        {
            return GetUuidAsync(name, timestamp).GetResult();
        }

        public static IEnumerable<MojangHistoryName> GetHistoryNames(string uuid)
        {
            return GetHistoryNamesAsync(uuid).GetResult();
        }

        public static IEnumerable<string> GetUuidsByNames(IEnumerable<string> names)
        {
            return GetUuidsByNamesAsync(names).GetResult();
        }

        public static MojangStatistics GetStatistics()
        {
            return GetStatisticsAsync().GetResult();
        }
    }
}