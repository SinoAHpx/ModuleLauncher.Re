using System.Diagnostics;
using CliWrap;
using CliWrap.Buffered;
using Manganese.Data;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.NET;

public class Launcher
{
    #region Exposed memebrs

    /// <summary>
    /// Minecraft resolver, you'll need to provide this only when you need to launch by minecraft id
    /// </summary>
    public MinecraftResolver MinecraftResolver { get; set; }

    public LauncherConfig LauncherConfig { get; set; } = new();

    /// <summary>
    /// Construct resolver via minecraft resolver
    /// </summary>
    /// <param name="minecraftResolver"></param>
    /// <param name="launcherConfig"></param>
    public Launcher(MinecraftResolver minecraftResolver, LauncherConfig launcherConfig)
    {
        MinecraftResolver = minecraftResolver;
        LauncherConfig = launcherConfig;
    }

    /// <summary>
    /// Construct libraries resolver via minecraft root path
    /// </summary>
    /// <param name="minecraftRootPath"></param>
    /// <param name="launcherConfig"></param>
    public Launcher(string? minecraftRootPath, LauncherConfig launcherConfig)
    {
        LauncherConfig = launcherConfig;
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

    /// <summary>
    /// Launch minecraft by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Process> LaunchAsync(string id)
    {
        var minecraftEntry = MinecraftResolver.GetMinecraft(id);

        return await LaunchAsync(minecraftEntry);
    }
    
    /// <summary>
    /// Launch minecraft
    /// </summary>
    /// <param name="minecraftEntry"></param>
    /// <param name="pipeTarget">How do you want to grab your output lines. No idea how to use it? Simply pass a null is allowed</param>
    /// <returns></returns>
    public async Task<CommandResult> LaunchAsync(MinecraftEntry minecraftEntry, PipeTarget? pipeTarget)
    {
        #region Precheck

        await WriteLauncherProfileAsync(minecraftEntry);
        
        #endregion
        
        var java = GetJava(minecraftEntry)?.Executable
            .ThrowIfNull(new InvalidJavaExecutableException("No java executable file was specified"));

        var arguments = GetLaunchArguments(minecraftEntry);
        var result = await Cli.Wrap(java
                .ThrowIfNull(new InvalidJavaExecutableException("No java executable file was specified"))
                .FullName)
            .WithArguments(arguments)
            .WithWorkingDirectory(minecraftEntry.Tree.WorkingDirectory.FullName)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(pipeTarget ?? PipeTarget.Null)
            .ExecuteAsync();
        
        await minecraftEntry.ExtractNativesAsync();
        await minecraftEntry.MapAssetsAsync();

        return result;
    }
    
    [Obsolete("This method will be soon abandoned")]
    public async Task<Process> LaunchAsync(MinecraftEntry minecraftEntry)
    {
        #region Precheck

        await WriteLauncherProfileAsync(minecraftEntry);
        
        #endregion
        
        var java = GetJava(minecraftEntry)?.Executable
            .ThrowIfNull(new InvalidJavaExecutableException("No java executable file was specified"));

        var arguments = GetLaunchArguments(minecraftEntry);

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = java?.FullName,
                Arguments = arguments,
                WorkingDirectory = minecraftEntry.Tree.WorkingDirectory.FullName,
                UseShellExecute = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        await minecraftEntry.ExtractNativesAsync();
        await minecraftEntry.MapAssetsAsync();

        process.Start();
            
        return process;
    }

    
    
    public string GetLaunchArguments(MinecraftEntry minecraftEntry)
    {
        var preset = GetJvmArguments(minecraftEntry);

        var raw = new List<string>
        {
            preset,
            minecraftEntry.Json.MainClass.ThrowCorruptedIfNull()
        };

        var minecraftArgs = GetMinecraftArguments(minecraftEntry);
        raw.Add(minecraftArgs);

        return raw.JoinToString(" ");
    }

    private MinecraftJava? GetJava(MinecraftEntry minecraftEntry)
    {
        var javaVersion = minecraftEntry.Json.JavaVersion;
        if (javaVersion == null)
        {
            if (minecraftEntry.HasInheritSource())
            {
                var inheritMinecraft = minecraftEntry.GetInheritSource()!;
                javaVersion = inheritMinecraft.Json.JavaVersion;
            }
            else
            {
                // for legacy loader
                javaVersion = 8;
            }
        }

        return LauncherConfig.Javas.FirstOrDefault(j => j.Version == javaVersion);
    }

    private async Task WriteLauncherProfileAsync(MinecraftEntry minecraftEntry)
    {
        var launcherProfilesFile = minecraftEntry.Tree.Root.DiveToFile("launcher_profiles.json");

        //write launcher_profiles.json if it doesn't exist
        if (!launcherProfilesFile.Exists)
        {
            await launcherProfilesFile.WriteAllTextAsync(new
            {
                selectedProfile = "(Default)",
                profiles = new
                {
                    Default = new
                    {
                        name = "(Default)"
                    }
                },
                clientToken = Guid.NewGuid()
            }.ToJsonString());
        }

    }

    private string GetMinecraftArguments(MinecraftEntry minecraftEntry)
    {
        var options = new List<string>();

        var boilerplate = GetMinecraftArgumentBoilerplate(minecraftEntry);
        
        //filling the boilerplate
        boilerplate = boilerplate.Replace("${auth_player_name}", $"\"{LauncherConfig.Authentication.Name}\"")
            .Replace("${version_name}", $"\"{LauncherConfig.LauncherName}\"")
            .Replace("${version_type}", $"\"{LauncherConfig.LauncherName}\"")
            .Replace("${game_directory}", $"\"{minecraftEntry.Tree.WorkingDirectory}\"")
            .Replace("${assets_root}", $"\"{minecraftEntry.Tree.Assets}\"")
            .ReplaceIfNull("${assets_index_name}", minecraftEntry.Json.AssetId,
                minecraftEntry.GetInheritSource()?.Json.AssetId)
            .Replace("${auth_uuid}", LauncherConfig.Authentication.UUID)
            .Replace("${auth_access_token}", LauncherConfig.Authentication.AccessToken)
            .Replace("${user_properties}", "{}")
            .Replace("${game_assets}", $"{minecraftEntry.Tree.Assets}")
            .Replace("${auth_session}", LauncherConfig.Authentication.AccessToken)
            .Replace("${user_type}", "msa");

        options.Add(boilerplate);

        if (LauncherConfig.Fullscreen is true)
            options.Add("--fullscreen");

        if (LauncherConfig.WindowHeight != null)
            options.Add($"--height {LauncherConfig.WindowHeight}");

        if (LauncherConfig.WindowWidth != null)
            options.Add($"--width {LauncherConfig.WindowWidth}");

        return options.JoinToString(" ");
    }

    private string GetMinecraftArgumentBoilerplate(MinecraftEntry minecraftEntry)
    {
        string boilerplate;
        //handle newer version
        if (minecraftEntry.Json.Arguments != null)
        {
            var gameArgs = minecraftEntry.Json.Arguments.FetchJToken("game").ThrowCorruptedIfNull();
            boilerplate = gameArgs
                .Where(x => x.Type == JTokenType.String)
                .Select(x => x.ToString())
                .Where(x => !x.ToLower().Contains("xuid") && !x.ToLower().Contains("clientid"))
                .JoinToString(" ");

            if (minecraftEntry.HasInheritSource())
            {
                boilerplate +=
                    $" {GetMinecraftArgumentBoilerplate(minecraftEntry.GetInheritSource()!)}";
            }
        }
        else
        {
            boilerplate =
                minecraftEntry.Json.MinecraftArguments.ThrowCorruptedIfNull();
        }

        return boilerplate;
    }
    
    private string GetJvmArguments(MinecraftEntry minecraftEntry)
    {
        var libraries = minecraftEntry.GetLibraries();
        
        var rawArgs = new List<string>
        {
            // "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump",
            "-XX:+UnlockExperimentalVMOptions",
            "-XX:+UseG1GC",
            "-XX:G1NewSizePercent=20",
            "-XX:G1ReservePercent=20",
            "-XX:MaxGCPauseMillis=50",
            "-XX:G1HeapRegionSize=32M",
            $"-Xmx{LauncherConfig.MaxMemorySize}M"
        };
    
        if (LauncherConfig.MinMemorySize != null)
            rawArgs.Add($"-Xmn{LauncherConfig.MinMemorySize}M");

        if (libraries.Any(l => l.IsNative))
            rawArgs.Add($"-Djava.library.path=\"{minecraftEntry.Tree.Natives}\"");
        else
            rawArgs.Add("-Djava.library.path=.");

        rawArgs.Add($"-cp \"{GetClassPath(minecraftEntry, libraries)}\"");

        if (minecraftEntry.Json.Arguments != null && minecraftEntry.GetMinecraftType() != MinecraftType.Vanilla)
        {
            var forgeArgs = minecraftEntry.Json.Arguments.FetchJToken("jvm")
                .ThrowCorruptedIfNull()
                .Where(x => x.Type == JTokenType.String)
                .Select(x => x.ToString())
                .Select(x =>
                    x.Replace(" ", "")
                        .Replace("${library_directory}", $"{minecraftEntry.Tree.Libraries}")
                        .Replace("${classpath_separator}", $"{Path.PathSeparator}")
                        .Replace("${version_name}", minecraftEntry.Json.Id));

            rawArgs.Add(forgeArgs.JoinToString(" "));
        }
        
        var args = rawArgs.JoinToString(" ");

        return args;
    }

    private string GetClassPath(MinecraftEntry minecraftEntry, List<LibraryEntry> libraries)
    {
        var raw = libraries
            .Where(l => !l.IsNative)
            .Select(l => minecraftEntry.Tree.Libraries.DiveToFile(l.RelativeUrl))
            .ToList();

        if (!minecraftEntry.HasInheritSource() || minecraftEntry.Tree.Jar.Exists)
            raw.Add(minecraftEntry.Tree.Jar);
        else
        {
            var inheritMinecraft = minecraftEntry.GetInheritSource();

            raw.Add(inheritMinecraft!.Tree.Jar);
        }

        var classpath = raw.JoinToString($"{Path.PathSeparator}");

        return classpath;
    }
}