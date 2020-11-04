using System.Collections.Generic;
using System.IO;
using System.Linq;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Locators
{
    public class MinecraftLocator
    {
        public string Location { get; set; }

        public MinecraftLocator(string location = null)
        {
            Location = location;
        }

        public IEnumerable<Minecraft> GetMinecrafts(bool readJson = false)
        {
            var dirs = Directory.GetDirectories($"{Location}\\versions");
            var re = new LinkedList<Minecraft>();
            foreach (var dir in dirs)
            {
                var files = Directory.GetFiles(dir);

                re.AddLast(new Minecraft
                {
                    File = new Minecraft.MinecraftFile
                    {
                        Jar = files.Any(x => x.IsJar()) ? new FileInfo(files.First(x => x.IsJar())) : null,
                        Json = new FileInfo(files.First(x => x.IsJson())),
                    },
                    Json = readJson ? JObject.Parse(File.ReadAllText(files.First(x => x.IsJson()))) : null
                });
            }

            return re;
        }
    }
}