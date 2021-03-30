using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Data.Locators
{
    public class Minecraft
    {
        /// <summary>
        /// Minecraft的本地目录信息描述
        /// </summary>
        public MinecraftFile File { get; set; }
        
        internal JToken OriginalJson { get; set; }
        
        /// <summary>
        /// Minecraft的json实体
        /// </summary>
        internal MinecraftJson Json { get; set; }
        internal MinecraftJson.MinecraftType Type { get; set; }
        
        /// <summary>
        /// 1.16.4 => 1.16
        /// </summary>
        internal string RootVersion { get; set; }
        
        /// <summary>
        /// loader类型的minecraft才会拥有此值
        /// </summary>
        internal Minecraft Inherit { get; set; }

        public class MinecraftFile
        {
            /// <summary>
            /// %version%/%version%.jar
            /// </summary>
            public FileInfo Jar { get; set; }
            
            /// <summary>
            /// %version%/%version%.json
            /// </summary>
            public FileInfo Json { get; set; }
            
            /// <summary>
            /// jar和json的父级目录
            /// </summary>
            public DirectoryInfo Version { get; set; }
            
            /// <summary>
            /// 存放natives的目录
            /// </summary>
            public DirectoryInfo Natives { get; set; }
            
            /// <summary>
            /// .minecraft/libraries
            /// </summary>
            public DirectoryInfo Libraries { get; set; }
            
            /// <summary>
            /// .minecraft/assets
            /// </summary>
            public DirectoryInfo Assets { get; set; }
            
            /// <summary>
            /// .minecraft/mods || %version%/mods
            /// </summary>
            public DirectoryInfo Mod { get; set; }
            
            /// <summary>
            /// .minecraft/saves || %version%/saves
            /// </summary>
            public DirectoryInfo Saves { get; set; }
            
            /// <summary>
            /// .minecraft/resourcepacks || %version%/resourcepacks
            /// </summary>
            public DirectoryInfo ResourcePacks { get; set; }
            
            /// <summary>
            /// .minecraft/texturepacks || %version%/texturepacks
            /// </summary>
            public DirectoryInfo TexturePacks { get; set; }
            
            /// <summary>
            /// .minecraft/shaderpacks || %version%/shaderpacks
            /// </summary>
            public DirectoryInfo ShaderPacks { get; set; }
            
            /// <summary>
            /// .minecraft || %version%
            /// </summary>
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