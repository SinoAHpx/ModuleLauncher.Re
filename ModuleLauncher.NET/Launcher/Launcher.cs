using System.Diagnostics;
using Manganese.Array;
using Manganese.Data;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Exceptions;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.NET.Launcher;

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
        //there're two types of arguments:
            // game and jvm
            // technically, in legacy versions, there's only game arguments as the content of 'minecraftArguments'
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

    private string GetJvmArguments(MinecraftEntry minecraftEntry)
    {
        var rawArgs = new List<string>
        {
            $"-Xmx{LauncherConfig.MaxMemorySize}M",
        };
        if (LauncherConfig.MinMemorySize != null)
            rawArgs.Add($"-Xmn{LauncherConfig.MinMemorySize}M");
        
        var libraries = minecraftEntry.GetLibraries();
        if (libraries.Any(l => l.IsNative))
            rawArgs.Add($"-Djava.library.path=\"{minecraftEntry.Tree.Natives}\"");
        else
            rawArgs.Add("-Djava.library.path=.");
        

        //if current version of Minecraft is legacy, this branch will not be executed at all.
        if (minecraftEntry.Json.Arguments != null)
        {
            var neoJvmArgs = minecraftEntry.Json.Arguments.FetchJToken("jvm")
                .ThrowCorruptedIfNull();
            foreach (var neoJvmArg in neoJvmArgs)
            {
                if (neoJvmArg.Type == JTokenType.String)
                {
                    var arg = neoJvmArg.ToString().Replace("${library_directory}", $"{minecraftEntry.Tree.Libraries}")
                        .Replace("${classpath_separator}", $"{Path.PathSeparator}")
                        .Replace("${version_name}", minecraftEntry.Json.Id)
                        .Replace("${natives_directory}", $"{minecraftEntry.Tree.Natives}")
                        .Replace("${launcher_name}", LauncherConfig.LauncherName)
                        .Replace("${launcher_version}", LauncherConfig.LauncherName);
                    
                    if (arg == "${classpath}")
                    {
                        arg = GetClassPath(minecraftEntry, libraries);
                    }
                    
                    rawArgs.Add(arg);
                }

                if (neoJvmArg.Type == JTokenType.Object)
                {
                    var rule = neoJvmArg.FetchJToken("rules")?.Single().ThrowCorruptedIfNull()!;
                    
                    //nobody gonna use a 32 bit system, so we simply neglect 'os.arch'
                    if (rule.Fetch("os.name") == CommonUtils.CurrentSystemName)
                    {
                        var value = neoJvmArg.FetchJToken("value").ThrowCorruptedIfNull();
                        if (value.Type == JTokenType.String)
                        {
                            rawArgs.Add(value.ToString());
                        }
                        else
                        {
                            value.ForEach(v => rawArgs.Add(v.ToString()));
                        }
                    }
                    
                }
            }
            

            // rawArgs.Add(neoJvmArgs.JoinToString(" "));
        }
        else
        {
            rawArgs.Add($"-classpath \"{GetClassPath(minecraftEntry, libraries)}\"");
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

        if (LauncherConfig.DirectlyJoinServer != null)
            options.Add($"--server {LauncherConfig.DirectlyJoinServer}");

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


}