using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using Newtonsoft.Json;

namespace AHpx.ModuleLauncher.Locators
{
    public class MinecraftLocator
    {
        public string Location { get; set; }

        public MinecraftLocator(string location = null)
        {
            Location = location;
        }

        public IEnumerable<Minecraft> GetMinecrafts(bool versionIsolation = true, bool readJson = false)
        {
            // var dirs = Directory.GetDirectories($"{Location}\\versions");
            // var re = new LinkedList<Minecraft>();
            // foreach (var dir in dirs)
            // {
            //     var files = Directory.GetFiles(dir);
            //
            //     re.AddLast(new Minecraft
            //     {
            //         File = new Minecraft.MinecraftFile
            //         {
            //             //Jar = files.Any(x => x.IsJar()) ? new FileInfo(files.First(x => x.IsJar())) : null,
            //             Jar = files.Any(x => x.EndsWith("jar")) ? new FileInfo(files.First(x => x.EndsWith("jar"))) : new FileInfo(""),
            //             Json = new FileInfo(files.First(x => x.IsJson())),
            //             Libraries = new DirectoryInfo($"{Location}\\libraries"),
            //             Assets = new DirectoryInfo($"{Location}\\assets"),
            //             Version = new DirectoryInfo(dir),
            //             Natives = new DirectoryInfo($"{dir}\\natives"),
            //             Minecraft = new DirectoryInfo(Location),
            //             Mod = versionIsolation ? new DirectoryInfo($"{dir}\\mods") : new DirectoryInfo($"{Location}\\mods"),
            //             ResourcePacks = versionIsolation ? new DirectoryInfo($"{dir}\\resourcepacks") : new DirectoryInfo($"{Location}\\resourcepacks"),
            //             TexturePacks = versionIsolation ? new DirectoryInfo($"{dir}\\texturepacks") : new DirectoryInfo($"{Location}\\texturepacks"),
            //             Saves = versionIsolation ? new DirectoryInfo($"{dir}\\saves") : new DirectoryInfo($"{Location}\\saves"),
            //             ShaderPacks = versionIsolation ? new DirectoryInfo($"{dir}\\shaderpacks") : new DirectoryInfo($"{Location}\\shaderpacks"),
            //         },
            //         Json = readJson 
            //             ? JsonConvert.DeserializeObject<Minecraft.MinecraftJson>(File.ReadAllText(files.First(x => x.IsJson()))) 
            //             : null
            //     });
            // }
            //
            // return re;
            
            //TODO: WE DON'T CARE IF IT DOESN'T EXIST LOL
            var dirs = Directory.GetDirectories($"{Location}\\versions").ToList();
            var result = new List<Minecraft>();
            dirs.ForEach(x =>
            {
                result.Add(new Minecraft
                {
                    File = new Minecraft.MinecraftFile
                    {
                        Jar = new FileInfo($@"{x}\{x.GetFileName()}.jar"),
                        Json = new FileInfo($@"{x}\{x.GetFileName()}.json"),
                        Version = new DirectoryInfo(x),
                        Assets = new DirectoryInfo($@"{Location}\assets"),
                        Libraries = new DirectoryInfo($@"{Location}\libraries"),
                        Root = versionIsolation ? new DirectoryInfo(x) : new DirectoryInfo(Location),
                        Mod = versionIsolation ? new DirectoryInfo($@"{x}\mods") : new DirectoryInfo($@"{Location}\mods"),
                        Natives = new DirectoryInfo($@"{x}\natives"),
                        ResourcePacks = versionIsolation ? new DirectoryInfo($@"{x}\resourcepacks") : new DirectoryInfo($@"{Location}\resourcepacks"),
                        TexturePacks = versionIsolation ? new DirectoryInfo($@"{x}\texturepacks") : new DirectoryInfo($@"{Location}\texturepacks"),
                        ShaderPacks = versionIsolation ? new DirectoryInfo($@"{x}\shaderpacks") : new DirectoryInfo($@"{Location}\shaderpacks"),
                        Saves = versionIsolation ? new DirectoryInfo($@"{x}\saves") : new DirectoryInfo($@"{Location}\saves")
                    }
                });
            });

            return result;
        }
        
        public Minecraft GetMinecraft(string version, bool isolation = true, bool readJson = false)
        {
            var minecrafts = GetMinecrafts(isolation, readJson).ToList();
            return minecrafts.FirstOrDefault(x => x.File.Json.Name.Contains(version));
        }

        internal Minecraft.MinecraftJson.MinecraftType GetMinecraftJsonType(string version)
        {
            var json = JsonConvert.DeserializeObject<Minecraft.MinecraftJson>(
                File.ReadAllText($@"{Location}\versions\{version}\{version}.json"), new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Include
                });
            
            var vDefault = new Version("1.7.10");
            var vNew = new Version("1.13");

            if (json?.Type == "snapshot")
            {
                try
                {
                    var ver = new Version(json.Assets);
                    return ver < vNew
                        ? Minecraft.MinecraftJson.MinecraftType.DefaultVanilla
                        : Minecraft.MinecraftJson.MinecraftType.NewVanilla;
                }
                catch
                {
                    return Minecraft.MinecraftJson.MinecraftType.OldVanilla;
                }
            }
            else
            {
                if (json?.Assets == null)
                {
                    if (json?.InheritsFrom != null)
                    {
                        var inherit = new Version(json.InheritsFrom!);
                        if (inherit < vDefault) return Minecraft.MinecraftJson.MinecraftType.OldLoader;

                        return inherit < vNew
                            ? Minecraft.MinecraftJson.MinecraftType.DefaultLoader
                            : Minecraft.MinecraftJson.MinecraftType.NewLoader;
                    }

                    return Minecraft.MinecraftJson.MinecraftType.OldLoader;
                }
                else
                {
                    if (json.Assets! == "legacy")
                    {
                        return json.InheritsFrom == null
                            ? Minecraft.MinecraftJson.MinecraftType.OldVanilla
                            : Minecraft.MinecraftJson.MinecraftType.OldLoader;
                    }
                    
                    return new Version(json.Assets) < vNew 
                        ? Minecraft.MinecraftJson.MinecraftType.DefaultVanilla
                        : Minecraft.MinecraftJson.MinecraftType.NewVanilla;
                }
            }
        }
    }
}