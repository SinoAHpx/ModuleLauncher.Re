using Manganese.Text;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;

namespace ModuleLauncher.NET;

public class Launcher
{
    #region Exposed memebrs

    /// <summary>
    /// Minecraft resolver, you'll need to provide this only when you need to launch by minecraft id
    /// </summary>
    public MinecraftResolver? MinecraftResolver { get; set; }

    /// <summary>
    /// Construct resolver via minecraft resolver
    /// </summary>
    /// <param name="minecraftResolver"></param>
    public Launcher(MinecraftResolver? minecraftResolver)
    {
        MinecraftResolver = minecraftResolver;
    }

    /// <summary>
    /// Construct libraries resolver via minecraft root path
    /// </summary>
    /// <param name="minecraftRootPath"></param>
    public Launcher(string? minecraftRootPath)
    {
        MinecraftResolver =
            minecraftRootPath.ThrowIfNullOrEmpty<NullReferenceException>("Root path of resolver could not be null");
    }

    /// <summary>
    /// Just an empty constructor
    /// </summary>
    public Launcher()
    {
    }

    #endregion
    
    public static string GetLaunchCommand(MinecraftEntry minecraftEntry)
    {
        return "";
    }
}