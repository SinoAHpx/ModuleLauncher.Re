using System.IO.Compression;
using Newtonsoft.Json;

namespace ModuleLauncher.NET.Models.Resources;

public sealed class MinecraftModEntry
{
    [JsonProperty(propertyName: "modid")]
    public string Id { get; set; }
    
    [JsonProperty(propertyName: "name")]
    public string Name { get; set; }
    
    [JsonProperty(propertyName: "description")]
    public string Description { get; set; }

    [JsonProperty(propertyName: "version")]
    public string Version { get; set; }
    
    [JsonProperty(propertyName: "mcversion")]
    public string McVersion { get; set; }
    
    [JsonProperty(propertyName: "logoFile")]
    public string LogoFileRelativeUrl { get; set; }
    
    [JsonProperty(propertyName: "updateUrl")]
    public string UpdateUrl { get; set; }

    [JsonProperty(propertyName: "authorList")]
    public List<string> Authors { get; set; }

    [JsonProperty(propertyName: "credits")]
    public string Credits { get; set; }
    
    [JsonProperty(propertyName: "parent")]
    public string Parent { get; set; }
    
    [JsonProperty(propertyName: "screenshots")]
    public List<string> Screenshots { get; set; }
    
    [JsonProperty(propertyName: "dependencies")]
    public List<string> Dependencies { get; set; }

    public bool IsEnabled
    {
        get => string.Compare(JarFile.Extension , ".jar", StringComparison.OrdinalIgnoreCase) == 0;
        set
        {
            if (IsEnabled == value) return;
            if (value)
            {
                File.Move(JarFile.FullName, JarFile.FullName[..JarFile.FullName.LastIndexOf('.')]);
            }
            else
            {
                File.Move(JarFile.FullName, JarFile.FullName+".DISABLED");
            }
        }
    }
    
    public FileInfo JarFile { get; set; }

    public static MinecraftModEntry Parse(FileInfo jarFile)
    {
        using var zipFile = new ZipArchive(new FileStream(jarFile.FullName, FileMode.Open), ZipArchiveMode.Read);
        using StreamReader reader = new(zipFile.GetEntry("mcmod.info").Open());
        var result = JsonConvert.DeserializeObject<List<MinecraftModEntry>>(reader.ReadToEnd()).First();
        result.JarFile = jarFile;
        return result;
    }
}