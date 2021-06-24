using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Launcher
{
    public class Launcher
    {
        private readonly MinecraftLocator _minecraftLocator;
        
        /// <summary>
        /// Path of java.exe or javaw.exe
        /// </summary>
        public string Java { get; set; }

        /// <summary>
        /// You launcher name, optioanl
        /// </summary>
        public string LauncherName { get; set; } = "ml.net";

        /// <summary>
        /// The unit is MB, you don't have to append a suffix,
        /// How much memory is allocated to jvm of minecraft at most, optional, default is 2
        /// </summary>
        public int MaximumMemorySize { get; set; }

        /// <summary>
        /// The unit is MB, you don't have to append a suffix,
        /// How much memory is allocated to jvm of minecraft at least, optional, default is null
        /// </summary>
        public int? MinimumMemorySize { get; set; }

        /// <summary>
        /// AuthenticateResult object, could be implicit convert via string 
        /// </summary>
        public AuthenticateResult Authentication { get; set; }

        /// <summary>
        /// Set the height of minecraft window, optional
        /// </summary>
        public int? WindowHeight { get; set; } = null;

        /// <summary>
        /// Set the width of minecraft window, optional
        /// </summary>
        public int? WindowWidth { get; set; } = null;

        /// <summary>
        /// Whether to play in fullscreen, optional
        /// </summary>
        public bool? Fullscreen { get; set; } = null;

        /// <summary>
        /// You can pass in a string directly
        /// </summary>
        /// <param name="minecraftLocator"></param>
        public Launcher(MinecraftLocator minecraftLocator)
        {
            _minecraftLocator = minecraftLocator;
        }

        /// <summary>
        /// Launch minecraft process via the set java path
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The process object</returns>
        public async Task<Process> Launch(string id)
        {
            var mc = await _minecraftLocator.GetLocalMinecraft(id);

            var argument = new StringBuilder();

            #region Jvm args

            argument.Append("-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump ");
            argument.Append("-XX:+UnlockExperimentalVMOptions ");
            argument.Append("-XX:+UseG1GC ");
            argument.Append("-XX:G1NewSizePercent=20 ");
            argument.Append("-XX:G1ReservePercent=20 ");
            argument.Append("-XX:MaxGCPauseMillis=50 ");
            argument.Append("-XX:G1HeapRegionSize=32M ");
            argument.Append($"-Djava.library.path=\"{mc.Locality.Natives}\" ");
            argument.Append($"-Xmx{MaximumMemorySize}M ");
            argument.Append(MinimumMemorySize != null ? $"-Xmn{MinimumMemorySize}M " : "");
            argument.Append(
                $"-classpath \"{string.Join(';', (await _minecraftLocator.GetLibraries(id)).Select(x => x.File))};");

            if (mc.IsInherit())
            {
                var inheritMc = await _minecraftLocator.GetLocalMinecraft(mc.Raw.InheritsFrom);
                
                argument.Append(inheritMc.Locality.Jar);
            }
            else
            {
                argument.Append(mc.Locality.Jar);
            }

            argument.Append("\" ");

            #endregion

            #region Minecraft args

            argument.Append($"{mc.Raw.MainClass} ");
            
            string minecraftArguments = string.Empty;

            if (mc.Raw.MinecraftArguments.IsNullOrEmpty())
            {
                if (mc.IsInherit())
                {
                    minecraftArguments += $"{(await mc.GetInherit()).Raw.Arguments.ToNormalArguments()} ";
                }
                
                minecraftArguments += mc.Raw.Arguments.ToNormalArguments();
            }
            else
            {
                minecraftArguments = mc.Raw.MinecraftArguments;
            }
            
            minecraftArguments = minecraftArguments.Replace("${auth_player_name}", $"\"{Authentication.Name}\"");
            minecraftArguments = minecraftArguments.Replace("${version_name}", $"\"{LauncherName}\"");
            minecraftArguments = minecraftArguments.Replace("${version_type}", $"\"{LauncherName}\"");
            minecraftArguments = minecraftArguments.Replace("${game_directory}", $"\"{mc.Locality.Root}\"");
            minecraftArguments = minecraftArguments.Replace("${assets_root}", $"\"{mc.Locality.Assets.Parent}\"");

            string assetId = mc.Raw.AssetId;
            if (mc.IsInherit())
            {
                var inheritMc = await _minecraftLocator.GetLocalMinecraft(mc.Raw.InheritsFrom);
                assetId = inheritMc.Raw.AssetId;
            }
            
            minecraftArguments = minecraftArguments.Replace("${assets_index_name}", $"\"{assetId}\"");
            minecraftArguments = minecraftArguments.Replace("${auth_uuid}", $"\"{Authentication.Uuid}\"");
            minecraftArguments = minecraftArguments.Replace("${auth_access_token}", $"\"{Authentication.AccessToken}\"");
            minecraftArguments = minecraftArguments.Replace("${user_properties}", "{}");
            minecraftArguments = minecraftArguments.Replace("${user_type}", "mojang");
            argument.Append($"{minecraftArguments} ");
            
            argument.Append(WindowWidth != null ? $"--width {WindowWidth} " : string.Empty);
            argument.Append(WindowHeight != null ? $"--height {WindowHeight} " : string.Empty);
            argument.Append(Fullscreen != null && Fullscreen.Value ? "--fullscreen " : string.Empty);

            #endregion

            Console.WriteLine(argument);
            
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Java,
                    Arguments = argument.ToString(),
                    WorkingDirectory = mc.Locality.Root.FullName,
                    UseShellExecute = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            await ExtractNatives(id);
            
            process.Start();
            
            return process;
        }

        private async Task ExtractNatives(string id)
        {
            var mc = await _minecraftLocator.GetLocalMinecraft(id);
            mc.Locality.Natives.Create();
            
            var natives = await _minecraftLocator.GetNatives(id);
            foreach (var dependency in natives)
            {
                var entries = ZipFile.OpenRead(dependency.File.FullName).Entries;
                
                foreach (var entry in entries)
                {
                    if (entry.FullName.EndsWith("dll"))
                    {
                        var file = mc.Locality.Natives.GetSubFileInfo(entry.FullName);
                        if (!file.Exists)
                        {
                            entry.ExtractToFile($@"{file}", true);
                        }
                    }
                }
            }
        }
    }
}