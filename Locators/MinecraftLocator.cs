using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Locators
{
    public partial class MinecraftLocator
    {
        public string Location { get; set; }

        public MinecraftLocator(string location = null)
        {
            Location = location;
        }

        /// <summary>
        /// 获取Location\versions下的所有Minecraft
        /// </summary>
        /// <param name="versionIsolation">是否版本隔离</param>
        /// <param name="readJson">是否读取json</param>
        /// <returns></returns>
        public IEnumerable<Minecraft> GetMinecrafts(bool versionIsolation = true)
        {
            var dirs = Directory.GetDirectories($"{Location}\\versions").ToList();
            var result = new List<Minecraft>();
            dirs.ForEach(x =>
            {
                result.Add(GetMinecraft(x.GetFileName(), versionIsolation));
            });

            return result;
        }

        /// <summary>
        /// 根据json获取指定的Minecraft对象
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="versionIsolation"></param>
        /// <returns></returns>
        public Minecraft GetMinecraft(JToken obj, bool versionIsolation = true)
        {
            var json = obj.ToObject<Minecraft.MinecraftJson>();
            var version = json.Id;
            
            var re = new Minecraft
            {
                File = new Minecraft.MinecraftFile
                {
                    Jar = new FileInfo($@"{Location}\versions\{version}\{version}.jar"),
                    Json = new FileInfo($@"{Location}\versions\{version}\{version}.json"),
                    Version = new DirectoryInfo($@"{Location}\versions\{version}"),
                    Assets = new DirectoryInfo($@"{Location}\assets"),
                    Libraries = new DirectoryInfo($@"{Location}\libraries"),
                    Root = versionIsolation
                        ? new DirectoryInfo($@"{Location}\versions\{version}")
                        : new DirectoryInfo(Location),
                    Mod = versionIsolation
                        ? new DirectoryInfo($@"{Location}\versions\{version}\mods")
                        : new DirectoryInfo($@"{Location}\mods"),
                    Natives = new DirectoryInfo($@"{Location}\versions\{version}\natives"),
                    ResourcePacks = versionIsolation
                        ? new DirectoryInfo($@"{Location}\versions\{version}\resourcepacks")
                        : new DirectoryInfo($@"{Location}\resourcepacks"),
                    TexturePacks = versionIsolation
                        ? new DirectoryInfo($@"{Location}\versions\{version}\texturepacks")
                        : new DirectoryInfo($@"{Location}\texturepacks"),
                    ShaderPacks = versionIsolation
                        ? new DirectoryInfo($@"{Location}\versions\{version}\shaderpacks")
                        : new DirectoryInfo($@"{Location}\shaderpacks"),
                    Saves = versionIsolation
                        ? new DirectoryInfo($@"{Location}\versions\{version}\saves")
                        : new DirectoryInfo($@"{Location}\saves")
                },
                Json = json,
                Type = GetMinecraftJsonType(json),
                RootVersion = GetMinecraftAssetIndex(json)
            };
            re.Inherit = GetInheritMinecraft(re, versionIsolation);
            
            return re;
        }
        
        /// <summary>
        /// 根据版本获取指定的Minecraft对象
        /// </summary>
        /// <param name="version"></param>
        /// <param name="versionIsolation"></param>
        /// <returns></returns>
        public Minecraft GetMinecraft(string version, bool versionIsolation = true)
        {
            var obj = JObject.Parse(File.ReadAllText($@"{Location}\versions\{version}\{version}.json"));

            return GetMinecraft(obj, versionIsolation);
        }

        public IEnumerable<Library> GetLibraries(string version, bool excludeNatives = true)
        {
            return GetLibraries(GetMinecraft(version), excludeNatives);
        }
        
        public IEnumerable<Library> GetLibraries(Minecraft mc, bool excludeNatives = true)
        {
            var re = new List<Library>();

            var libs = mc.Json.Libraries;
            
            libs.Where(x => IsAllow(x) && !IsNative(x)).ForEach(x =>
            {
                re.Add(new Library
                {
                    File = new FileInfo(@$"{Location}\libraries\{x["name"].ToString().ToLibraryFile()}"),
                    Name = x["name"].ToString()
                });
            });
            
            if (mc.Type.IsLoader())
            {
                re.AddRange(GetLibraries(mc.Inherit.File.Version.Name));
            }

            return excludeNatives ? re : re.Concat(GetNatives(mc));
        }

        public IEnumerable<Library> GetNatives(string version)
        {
            return GetNatives(GetMinecraft(version));
        }
        
        public IEnumerable<Library> GetNatives(Minecraft mc)
        {
            var re = new List<Library>();

            var libs = mc.Json.Libraries;
            
            libs.Where(x => IsAllow(x) && IsNative(x)).ForEach(x =>
            {
                var suffix = x["natives"]["windows"].ToString()
                    .Replace("${arch}", Directory.Exists(@"C:\Program Files (x86)") ? "64" : "32");
                
                re.Add(new Library
                {
                    File = new FileInfo($@"{Location}\libraries\{x["name"].ToString().ToLibraryFile(false)}-{suffix}.jar"),
                    Name = x["name"].ToString()
                });
            });
            
            if (mc.Type.IsLoader())
            {
                re.AddRange(GetNatives(mc.Inherit.File.Version.Name));
            }

            return re;
        }

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

    public partial class MinecraftLocator
    {
        private static string GetMinecraftAssetIndex(Minecraft.MinecraftJson json)
        {
            //if it has a assets property
            try
            {
                var ver = new Version(json.Assets);

                return ver.ToString();
            }
            catch
            {
                //if it has a inheritsFrom property
                try
                {
                    Version.Parse(json.InheritsFrom);
                    var split = json.InheritsFrom.Split('.');
                    return $"{split[0]}.{split[1]}";
                }
                //if it has nothing
                catch
                {
                    try
                    {
                        var split = json.Id.RemoveAlphabets().Split('.');
                        var ver = new Version($"{split[0]}.{split[1]}");

                        return ver < new Version("1.6")
                            ? "pre-1.6"
                            : "legacy";
                    }
                    catch
                    {
                        return json.Assets;
                    }
                }
            }
        }

        private static Minecraft.MinecraftJson.MinecraftType GetMinecraftJsonType(Minecraft.MinecraftJson json)
        {
            if (json == null) throw new ArgumentException("Minecraft json data is null", nameof(json)); 
            
            var defaultVersion = new Version("1.7.10");
            var newVersion = new Version("1.13");

            //trying to parse inheritsFrom property
            try
            {
                var ver = new Version(json.InheritsFrom);
                if (ver < defaultVersion)
                    throw new Exception("Unsupported version type!");
                if (ver < newVersion && ver >= defaultVersion)
                    return Minecraft.MinecraftJson.MinecraftType.DefaultLoader;
                if (ver >= newVersion)
                    return Minecraft.MinecraftJson.MinecraftType.NewLoader;
            }
            //if failed, it's vanilla or old loader
            catch
            {
                //trying to parse assets property
                try
                {
                    if (json.Assets == "pre-1.6" || json.Assets == "legacy")
                        return Minecraft.MinecraftJson.MinecraftType.OldVanilla;
                    var ver = new Version(json.Assets);
                    if (ver < defaultVersion)
                        return Minecraft.MinecraftJson.MinecraftType.OldVanilla;
                    if (ver < newVersion && ver >= defaultVersion)
                        return Minecraft.MinecraftJson.MinecraftType.DefaultVanilla;
                    if (ver >= newVersion)
                        return Minecraft.MinecraftJson.MinecraftType.NewVanilla;
                }
                //if failed, trying to parse id property
                catch
                {
                    try
                    {
                        var ver = new Version(json.Id);
                        if (ver < defaultVersion)
                            return Minecraft.MinecraftJson.MinecraftType.OldVanilla;
                        if (ver < newVersion && ver >= defaultVersion)
                            return Minecraft.MinecraftJson.MinecraftType.DefaultVanilla;
                        if (ver >= newVersion)
                            return Minecraft.MinecraftJson.MinecraftType.NewVanilla;
                    }
                    catch
                    {
                        throw new Exception("Unsupported version type!");
                    }
                }
                
            }
            
            throw new ArgumentException("Version parse failed", nameof(json));
        }

        private Minecraft GetInheritMinecraft(Minecraft mc, bool isolation = true)
        {
            var json = mc.Json;

            return mc.Type.IsLoader() ? GetMinecraft(json.InheritsFrom, isolation) : mc;
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