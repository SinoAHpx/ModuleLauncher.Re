using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Utils;
using AHpx.ModuleLauncher.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Utils.Network
{
    public class MojangApi
    {
        public static async Task<IEnumerable<HistoryName>> GetHistoryNames(string uuid)
        {
            var url = $"https://api.mojang.com/user/profiles/{uuid}/names";
            var names = JArray.Parse((await HttpUtils.Get(url)).Content);
            var re = new List<HistoryName>();
            names.ForEach(x =>
            {
                re.Add(new HistoryName
                {
                    Name = x["name"].ToString(),
                    ChangedAt = x["changedToAt"]?.ToString()
                });
            });

            return re;
        }
    }
}