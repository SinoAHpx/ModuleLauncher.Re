namespace ModuleLauncher.NET.Models.Resources;

public class MinecraftEntry
{
    /// <summary>
    /// Minecraft json entity
    /// </summary>
    public MinecraftJson Json { get; set; }

    /// <summary>
    /// Minecraft files tree structure
    /// </summary>
    public MinecraftTree Tree { get; set; }
}