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
            //TODO: WE DON'T CARE IF IT DOESN'T EXIST LOL
            var dirs = Directory.GetDirectories($"{Location}\\versions").ToList();
            var result = new List<Minecraft>();
            dirs.ForEach(x =>
            {
                result.Add(GetMinecraft(x.GetFileName(), versionIsolation, readJson));
            });

            return result;
        }
        
        public Minecraft GetMinecraft(string version, bool versionIsolation = true, bool readJson = true)
        {
            var json = readJson
                ? JsonConvert.DeserializeObject<Minecraft.MinecraftJson>(
                    File.ReadAllText($@"{Location}\versions\{version}\{version}.json"))
                : null;
            
            return new Minecraft
            {
                File = new Minecraft.MinecraftFile
                {
                    Jar = new FileInfo($@"{Location}\versions\{version}\{version}.jar"),
                    Json = new FileInfo($@"{Location}\versions\{version}\{version}.json"),
                    Version = new DirectoryInfo($@"{Location}\versions\{version}"),
                    Assets = new DirectoryInfo($@"{Location}\assets"),
                    Libraries = new DirectoryInfo($@"{Location}\libraries"),
                    Root = versionIsolation ? new DirectoryInfo($@"{Location}\versions\{version}") : new DirectoryInfo(Location),
                    Mod = versionIsolation ? new DirectoryInfo($@"{Location}\versions\{version}\mods") : new DirectoryInfo($@"{Location}\mods"),
                    Natives = new DirectoryInfo($@"{Location}\versions\{version}\natives"),
                    ResourcePacks = versionIsolation ? new DirectoryInfo($@"{Location}\versions\{version}\resourcepacks") : new DirectoryInfo($@"{Location}\resourcepacks"),
                    TexturePacks = versionIsolation ? new DirectoryInfo($@"{Location}\versions\{version}\texturepacks") : new DirectoryInfo($@"{Location}\texturepacks"),
                    ShaderPacks = versionIsolation ? new DirectoryInfo($@"{Location}\versions\{version}\shaderpacks") : new DirectoryInfo($@"{Location}\shaderpacks"),
                    Saves = versionIsolation ? new DirectoryInfo($@"{Location}\versions\{version}\saves") : new DirectoryInfo($@"{Location}\saves")
                },
                Json = json,
                Type = GetMinecraftJsonType(json)
            };
        }

        private Minecraft.MinecraftJson.MinecraftType GetMinecraftJsonType(Minecraft.MinecraftJson json)
        {
            var defaultVersion = new Version("1.7.10");
            var newVersion = new Version("1.13");

            //trying to parse inheritsFrom property
            try
            {
                var ver = new Version(json.InheritsFrom);
                if (ver < defaultVersion)
                    return Minecraft.MinecraftJson.MinecraftType.OldLoader;
                if (ver < newVersion && ver >= defaultVersion)
                    return Minecraft.MinecraftJson.MinecraftType.DefaultLoader;
                if (ver >= newVersion)
                    return Minecraft.MinecraftJson.MinecraftType.NewLoader;
            }
            //if failed, it's vanilla or old loader
            catch (Exception e)
            {
                //trying to parse assets property
                try
                {
                    //TODO:IGNORE THE MODIFIED CLIENTS
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
                    catch (Exception exception)
                    {
                        return Minecraft.MinecraftJson.MinecraftType.OldLoader;
                    }
                }
                
            }
            
            throw new ArgumentException("Version parse failed", nameof(json));
        }
    }
}