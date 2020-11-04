using System.IO;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Data.Locators
{
    public class Minecraft
    {
        public MinecraftFile File { get; set; }
        internal JObject Json { get; set; }
        
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
            public DirectoryInfo Minecraft { get; set; }
        }
    }
}