using System.Collections;
using System.Collections.Generic;
using System.IO;
using AHpx.ModuleLauncher.Data.Locators;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Locators
{
    public class AssetsLocator : MinecraftLocator
    {
        public AssetsLocator(string location = null) : base(location) { }

        public IEnumerable<Asset> GetAssets(string version)
        {
            return GetAssets(GetMinecraft(version));
        }
        
        public IEnumerable<Asset> GetAssets(Minecraft mc)
        {
            var re = new List<Asset>();

            var json = JObject.Parse(File.ReadAllText($@"{mc.File.Assets}\indexes\{mc.RootVersion}.json"));
            var table = json["objects"].ToObject<Hashtable>();
            foreach (DictionaryEntry o in table)
            {
                var obj = JObject.Parse(o.Value.ToString());
                var hash = obj["hash"].ToString();

                re.Add(new Asset
                {
                    File = new FileInfo($@"{mc.File.Assets}\objects\{hash.Substring(0, 2)}\{hash}")
                });
            }
            
            return re;
        }
    }
}