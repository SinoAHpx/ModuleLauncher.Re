using System.Collections.Generic;
using System.IO;
using System.Linq;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Minecraft.Locator;
using Newtonsoft.Json;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    //head
    public partial class MinecraftLocator
    {
        public string Location { get; set; }

        public MinecraftLocator(string location = ".\\")
        {
            Location = location;
        }
        
        public static implicit operator MinecraftLocator(string location)
        {
            return new MinecraftLocator(location);
        }
    }
    
    //exposed
    public partial class MinecraftLocator
    {
        /// <summary>
        /// 获取versions目录下所有的Minecraft版本
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MinecraftFileEntity> GetMinecrafts()
        {
            return Directory.GetDirectories($"{Location}\\versions").Select(s => new MinecraftFileEntity
            {
                Jar = $"{s}\\{Path.GetFileName(s)}.jar",
                Json = $"{s}\\{Path.GetFileName(s)}.json",
                Native = $"{s}\\{Path.GetFileName(s)}-natives",
                Root = s
            });
        }
        
        /// <summary>
        /// 获取指定的Minecraft版本
        /// </summary>
        /// <param name="name">版本文件名</param>
        /// <returns></returns>
        public MinecraftFileEntity GetMinecraft(string name)
        {
            return new MinecraftFileEntity
            {
                Jar = $"{Location}\\versions\\{name}\\{name}.jar",
                Json = $"{Location}\\versions\\{name}\\{name}.json",
                Native = $"{Location}\\versions\\{name}\\{name}-natives",
                Root = $"{Location}\\versions\\{name}"
            };
        }
    }
    
    //inside
    public partial class MinecraftLocator
    {
        /// <summary>
        /// 解析指定Minecraft版本的json文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal MinecraftJsonEntity GetMinecraftEntity(string name)
        {
            var mcFolder = $"{Location}\\versions\\{name}";
            if (File.Exists($"{mcFolder}\\{name}.json"))
                return JsonConvert.DeserializeObject<MinecraftJsonEntity>($"{mcFolder}\\{name}.json");
            
            foreach (var file in Directory.GetFiles(mcFolder))
                if (Path.GetFileName(file).EndsWith("json"))
                    return JsonConvert.DeserializeObject<MinecraftJsonEntity>(file);

            return JsonConvert.DeserializeObject<MinecraftJsonEntity>($"{mcFolder}\\{name}.json");
        }
        
        /// <summary>
        /// 解析所有versions目录下Minecraft的json文件
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<MinecraftJsonEntity> GetMinecraftEntities()
        {
            var re = new List<MinecraftJsonEntity>();
            
            GetMinecrafts().Select(x => x.Json).ForEach(x =>
            {
                if (File.Exists(x))
                    re.Add(JsonConvert.DeserializeObject<MinecraftJsonEntity>(File.ReadAllText(x)));
                else
                    re.AddRange(
                        from file 
                        in Directory.GetFiles($"{Location}\\versions\\{Path.GetFileNameWithoutExtension(x)}") 
                        where Path.GetFileName(file).EndsWith("json") 
                        select JsonConvert.DeserializeObject<MinecraftJsonEntity>(file));
            });

            return re;
        }
    }
}