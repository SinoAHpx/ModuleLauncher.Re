﻿using ModuleLauncher.NET.Models.Authentication;

namespace ModuleLauncher.NET.Models.Launcher;

public class LauncherConfig
{
    /// <summary>
    /// Java executable files
    /// </summary>
    public List<MinecraftJava> Javas { get; set; }

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
    public FileInfo Executable { get; set; }

    /// <summary>
    /// Version of java runtime
    /// <example>8, 16, 17</example>
    /// </summary>
    public int Version { get; set; }
}