using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using Newtonsoft.Json;
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

        public IEnumerable<Minecraft> GetMinecrafts(bool versionIsolation = true, bool readJson = true)
        {
            var dirs = Directory.GetDirectories($"{Location}\\versions").ToList();
            var result = new List<Minecraft>();
            dirs.ForEach(x =>
            {
                result.Add(GetMinecraft(x.GetFileName(), versionIsolation, readJson));
            });

            return result;
        }

        public Minecraft GetMinecraft(JObject obj)
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
                    Root = new DirectoryInfo(Location),
                    Mod = new DirectoryInfo($@"{Location}\mods"),
                    Natives = new DirectoryInfo($@"{Location}\versions\{version}\natives"),
                    ResourcePacks = new DirectoryInfo($@"{Location}\resourcepacks"),
                    TexturePacks = new DirectoryInfo($@"{Location}\texturepacks"),
                    ShaderPacks = new DirectoryInfo($@"{Location}\shaderpacks"),
                    Saves = new DirectoryInfo($@"{Location}\saves")
                },
                Json = json,
                Type = GetMinecraftJsonType(json),
                RootVersion = GetMinecraftAssetIndex(json)
            };
            re.Inherit = GetInheritMinecraft(re);
            
            return re;
        }
        
        public Minecraft GetMinecraft(string version, bool versionIsolation = true, bool readJson = true)
        {
            var json = readJson
                ? JsonConvert.DeserializeObject<Minecraft.MinecraftJson>(
                    File.ReadAllText($@"{Location}\versions\{version}\{version}.json"))
                : null;
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
            re.Inherit = GetInheritMinecraft(re, versionIsolation, readJson);
            
            return re;
        }

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
                    //TODO:IGNORE THE MODIFIED CLIENTS
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

        private Minecraft GetInheritMinecraft(Minecraft mc, bool isolation = true, bool readJson = true)
        {
            var json = mc.Json;
            switch (GetMinecraftJsonType(json))
            {
                case Minecraft.MinecraftJson.MinecraftType.DefaultLoader:
                case Minecraft.MinecraftJson.MinecraftType.NewLoader:
                    return GetMinecraft(json.InheritsFrom, isolation, readJson);
                case Minecraft.MinecraftJson.MinecraftType.DefaultVanilla:
                case Minecraft.MinecraftJson.MinecraftType.NewVanilla:
                case Minecraft.MinecraftJson.MinecraftType.OldVanilla:
                    return mc;
                default:
                    throw new Exception("Unsupported version type!");
            }
        }
    }
}