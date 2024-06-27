using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Models.Launcher;

public class LauncherConfig
{
    /// <summary>
    /// Java executable files, default value is an empty list
    /// </summary>
    public List<MinecraftJava> Javas { get; set; } = new();

    /// <summary>
    /// You launcher name, optional
    /// </summary>
    public string LauncherName { get; set; } = "ml.net";

    /// <summary>
    /// The unit is MB, you don't have to append a suffix,
    /// How much memory is allocated to jvm of minecraft at most, optional, default is 1024
    /// </summary>
    public int MaxMemorySize { get; set; } = 1024;

    /// <summary>
    /// The unit is MB, you don't have to append a suffix,
    /// How much memory is allocated to jvm of minecraft at least, optional, default is null
    /// </summary>
    public int? MinMemorySize { get; set; }

    /// <summary>
    /// The example pattern(server:port): 127.0.0.1:8080. Invalid pattern will occur an error.
    /// </summary>
    public string? DirectlyJoinServer { get; set; }

    /// <summary>
    /// AuthenticateResult object, could be implicitly convert via string 
    /// </summary>
    public AuthenticateResult Authentication { get; set; }

    /// <summary>
    /// Set the height of minecraft window, optional
    /// </summary>
    public int? WindowHeight { get; set; }

    /// <summary>
    /// Set the width of minecraft window, optional
    /// </summary>
    public int? WindowWidth { get; set; }

    /// <summary>
    /// Whether to play in fullscreen, optional
    /// </summary>
    public bool? Fullscreen { get; set; }
}

public class MinecraftJava
{
    /// <summary>
    /// File info of java executable file
    /// <example>javaw.exe or java.exe</example>
    /// </summary>
    public FileInfo? Executable { get; set; }

    /// <summary>
    /// Version of java runtime
    /// <example>8, 16, 17</example>
    /// </summary>
    public int Version { get; set; }
    
    /// <summary>
    /// Initialize MinecraftJava object from executable file path
    /// </summary>
    /// <returns></returns>
    public static MinecraftJava? Of(string path)
    {
        var file = new FileInfo(path);
        var version = file.GetJavaExecutableVersion();

        if (version != null)
            return new MinecraftJava
            {
                Executable = file,
                Version = version.Value
            };

        return null;
    }
}