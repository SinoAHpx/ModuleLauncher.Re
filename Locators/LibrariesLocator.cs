using System.Collections.Generic;
using System.IO;
using System.Linq;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Locators
{
    public class LibrariesLocator : MinecraftLocator
    {
        public LibrariesLocator(string location = null) : base(location){}

        public IEnumerable<Library> GetLibraries(string version)
        {
            var re = new List<Library>();
            var libs = GetMinecraft(version).Json.Libraries;

            libs.Where(x => IsAllow(x) && !IsNative(x)).ForEach(x =>
            {
                re.Add(new Library
                {
                    File = new FileInfo(x["name"].ToString().ToLibraryFile()),
                    Name = x["name"].ToString()
                });
            });

            return re;
        }

        private bool IsAllow(JToken token)
        {
            var obj = token.ToObject<JObject>();
            if (obj.ContainsKey("rules"))
            {
                foreach (var jToken in obj["rules"].ToObject<JArray>())
                {
                    var o = jToken.ToObject<JObject>();
                    if (o["action"].ToString() == "allow")
                    {
                        if (o.ContainsKey("os"))
                        {
                            return o["os"]["name"].ToString().Contains("windows");
                        }

                        return true;
                    }
                    
                    if (o.ContainsKey("os"))
                    {
                        return !o["os"]["name"].ToString().Contains("windows");
                    }

                    return false;
                }
            }

            return true;
        }

        private bool IsNative(JToken token)
        {
            var obj = token.ToObject<JObject>();

            return obj.ContainsKey("natives");
        }
    }
}