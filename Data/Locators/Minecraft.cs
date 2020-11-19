using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Data.Locators
{
    public class Minecraft
    {
        public MinecraftFile File { get; set; }
        internal MinecraftJson Json { get; set; }
        internal MinecraftJson.MinecraftType Type { get; set; }
        internal string RootVersion { get; set; }
        internal Minecraft Inherit { get; set; }

        public class MinecraftFile
        {
            public FileInfo Jar { get; set; }
            public FileInfo Json { get; set; }
            public DirectoryInfo Version { get; set; }
            public DirectoryInfo Natives { get; set; }
            public DirectoryInfo Libraries { get; set; }
            public DirectoryInfo Assets { get; set; }
            public DirectoryInfo Mod { get; set; }
            public DirectoryInfo Saves { get; set; }
            public DirectoryInfo ResourcePacks { get; set; }
            public DirectoryInfo TexturePacks { get; set; }
            public DirectoryInfo ShaderPacks { get; set; }
            public DirectoryInfo Root { get; set; }
            
            public override string ToString()
            {
                var props = this.GetType().GetProperties();

                return props.Where(info => info.GetAccessors(false)[0].IsPublic).Aggregate(string.Empty,
                    (current, info) => current + $"PROP_NAME:{info.Name}\tPROP_VALUE:{info.GetValue(this)}\n");
            }
        }

        internal class MinecraftJson
        {
            [JsonProperty("assetIndex")]
            internal JObject AssetIndex { get; set; }
            
            [JsonProperty("assets")]
            internal string Assets { get; set; }
            
            [JsonProperty("downloads")]
            internal JObject Downloads { get; set; }
            
            [JsonProperty("arguments")]
            internal JObject Arguments { get; set; }
            
            [JsonProperty("minecraftArguments")]
            internal string MinecraftArguments { get; set; }
            
            [JsonProperty("id")]
            internal string Id { get; set; }
            
            [JsonProperty("libraries")]
            internal JArray Libraries { get; set; }
            
            [JsonProperty("type")]
            internal string Type { get; set; }
            
            [JsonProperty("inheritsFrom")]
            internal string InheritsFrom { get; set; }
            
            [JsonProperty("mainClass")]
            internal string MainClass { get; set; }
            
            internal enum MinecraftType
            {
                DefaultVanilla,
                NewVanilla,
                OldVanilla,
                DefaultLoader,
                NewLoader
            }
        }

        public override string ToString()
        {
            var props = this.GetType().GetProperties();
            var re = string.Empty;
            
            foreach (var info in props)
            {
                if (info.GetAccessors(false)[0].IsPublic)
                {
                    re += $"PROP_NAME:{info.Name}\nPROP_VALUE:\n{info.GetValue(this)}\n";
                }
            }

            return re;
        }
    }
}