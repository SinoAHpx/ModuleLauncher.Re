namespace ModuleLauncher.NET.Mods.Models.Utils;

public class ModInfo
{
    public string? Name { get; set; }

    public string? Id { get; set; }

    public string? Description { get; set; }

    public string? Version { get; set; }
    
    public string? License { get; set; }

    public List<string>? Authors { get; set; }
}