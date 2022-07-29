using System.Diagnostics;
using Manganese.Text;
using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;

namespace ModuleLauncher.NET.Utilities;

public static class LauncherUtils
{
    /// <summary>
    /// LauncherUtils method chain middleware
    /// </summary>
    public class LauncherConfigBuilder
    {
        internal LauncherConfig LauncherConfig { get; set; } = null!;
        internal MinecraftEntry MinecraftEntry { get; set; } = null!;
    }
    
    /// <summary>
    /// Build LauncherConfig with Authentication
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithAuthentication(this MinecraftEntry minecraftEntry,
        AuthenticateResult result)
    {
        var builder = new LauncherConfigBuilder
        {
            LauncherConfig = new LauncherConfig
            {
                Authentication = result
            },
            MinecraftEntry = minecraftEntry
        };

        return builder;
    }

    /// <summary>
    /// Append launcher name
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithLauncherName(this MinecraftEntry minecraftEntry, string? name)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
        };
        if (!name.IsNullOrEmpty())
        {
            builder.LauncherConfig.LauncherName = name;
        }

        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithMaxMemorySize(this MinecraftEntry minecraftEntry, int? size)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
            LauncherConfig = new LauncherConfig()
        };
        if (size != null)
        {
            builder.LauncherConfig.MaxMemorySize = size.Value;
        }

        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithMinMemorySize(this MinecraftEntry minecraftEntry, int? size)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
            LauncherConfig = new LauncherConfig
            {
                MinMemorySize = size
            }
        };


        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithWindowHeight(this MinecraftEntry minecraftEntry, int? height)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
            LauncherConfig = new LauncherConfig
            {
                WindowHeight = height
            }
        };

        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithWindowWidth(this MinecraftEntry minecraftEntry, int? width)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
            LauncherConfig = new LauncherConfig
            {
                WindowWidth = width
            }
        };

        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="state">Use fullscreen or not, default is true</param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithFullscreen(this MinecraftEntry minecraftEntry, bool state = true)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
            LauncherConfig = new LauncherConfig
            {
                Fullscreen = state
            }
        };
        return builder;
    }


    /// <summary>
    /// Append single minecraft java
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="java"></param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithJava(this MinecraftEntry minecraftEntry, MinecraftJava? java)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
            LauncherConfig = new LauncherConfig()
        };
        if (java != null)
        {
            builder.LauncherConfig.Javas.Add(java);
        }

        return builder;
    }


    /// <summary>
    /// Append multiple javas
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="javas"></param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithJavas(this MinecraftEntry minecraftEntry, List<MinecraftJava>? javas)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
            LauncherConfig = new LauncherConfig()
        };
        if (javas != null)
        {
            builder.LauncherConfig.Javas.AddRange(javas);
        }

        return builder;
    }


    /// <summary>
    /// Append java with java executable file path, ModuleLauncher will try to automatically get the version of java exe
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="javaExePath">Java executable file path</param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithJava(this MinecraftEntry minecraftEntry, string? javaExePath)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
            LauncherConfig = new LauncherConfig()
        };
        if (!javaExePath.IsNullOrEmpty())
        {
            builder.LauncherConfig.Javas.Add(MinecraftJava.Of(javaExePath));
        }

        return builder;
    }


    /// <summary>
    /// Multiple java paths
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="javaExePaths"></param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithJavas(this MinecraftEntry minecraftEntry, List<string?>? javaExePaths)
    {
        var builder = new LauncherConfigBuilder
        {
            MinecraftEntry = minecraftEntry,
            LauncherConfig = new LauncherConfig()
        };
        if (javaExePaths != null)
        {
            foreach (var path in javaExePaths)
            {
                if (path.IsNullOrEmpty())
                    continue;

                builder.LauncherConfig.Javas.Add(MinecraftJava.Of(path));
            }
        }

        return builder;
    }

    /// <summary>
    /// Build LauncherConfig with Authentication
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithAuthentication(this LauncherConfigBuilder builder,
        AuthenticateResult result)
    {
        builder.LauncherConfig.Authentication = result;

        return builder;
    }

    /// <summary>
    /// Append launcher name
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithLauncherName(this LauncherConfigBuilder builder, string? name)
    {
        if (!name.IsNullOrEmpty())
        {
            builder.LauncherConfig.LauncherName = name;
        }

        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithMaxMemorySize(this LauncherConfigBuilder builder, int? size)
    {
        if (size != null)
        {
            builder.LauncherConfig.MaxMemorySize = size.Value;
        }

        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithMinMemorySize(this LauncherConfigBuilder builder, int? size)
    {
        builder.LauncherConfig.MinMemorySize = size;

        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithWindowHeight(this LauncherConfigBuilder builder, int? height)
    {
        builder.LauncherConfig.WindowHeight = height;

        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithWindowWidth(this LauncherConfigBuilder builder, int? width)
    {
        builder.LauncherConfig.WindowWidth = width;

        return builder;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithFullscreen(this LauncherConfigBuilder builder, bool state = true)
    {
        builder.LauncherConfig.Fullscreen = state;
        return builder;
    }


    /// <summary>
    /// Append single minecraft java
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithJava(this LauncherConfigBuilder builder, MinecraftJava? java)
    {
        if (java != null)
        {
            builder.LauncherConfig.Javas.Add(java);
        }

        return builder;
    }


    /// <summary>
    /// Append multiple javas
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithJavas(this LauncherConfigBuilder builder, List<MinecraftJava>? javas)
    {
        if (javas != null)
        {
            builder.LauncherConfig.Javas.AddRange(javas);
        }

        return builder;
    }


    /// <summary>
    /// Append java with java executable file path, ModuleLauncher will try to automatically get the version of java exe
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="javaExePath">Java executable file path</param>
    /// <returns></returns>
    public static LauncherConfigBuilder WithJava(this LauncherConfigBuilder builder, string? javaExePath)
    {
        if (!javaExePath.IsNullOrEmpty())
        {
            builder.LauncherConfig.Javas.Add(MinecraftJava.Of(javaExePath));
        }

        return builder;
    }


    /// <summary>
    /// Multiple java paths
    /// </summary>
    /// <returns></returns>
    public static LauncherConfigBuilder WithJavas(this LauncherConfigBuilder builder, List<string?>? javaExePaths)
    {
        if (javaExePaths != null)
        {
            foreach (var path in javaExePaths)
            {
                if (path.IsNullOrEmpty())
                    continue;

                builder.LauncherConfig.Javas.Add(MinecraftJava.Of(path));
            }
        }

        return builder;
    }

    /// <summary>
    /// Launch minecraft from internal config, should be the last item of the method chain
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static async Task<Process> LaunchAsync(this MinecraftEntry minecraftEntry, LauncherConfig config)
    {
        var launcher = new Launcher
        {
            MinecraftResolver = MinecraftResolver.Of(minecraftEntry),
            LauncherConfig = config
        };

        return await launcher.LaunchAsync(minecraftEntry);
    }

    public static async Task<Process> LaunchAsync(this LauncherConfigBuilder builder)
    {
        return await builder.MinecraftEntry.LaunchAsync(builder.LauncherConfig);
    }
}