using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using AHpx.ModuleLauncher.Data.Authentications;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Launcher
{
    public class Launcher
    {

        private MinecraftLocator _locator;
        public MinecraftLocator Locator
        {
            get => _locator;
            set => _locator = value ?? new MinecraftLocator(@".\.minecraft");
        }

        public AuthenticateResult Auth { get; set; }

        public string JavaPath { get; set; }

        private string _launcherName;
        public string LauncherName
        {
            get => _launcherName;
            set => _launcherName = value ?? "ModuleLauncher";
        }
        
        private byte? _maxMemorySize;
        public byte? MaxMemorySize
        {
            get => _maxMemorySize;
            set
            {
                if (value <= 0)
                {
                    _maxMemorySize = 2;
                }

                _maxMemorySize = value ?? 2;
            }
        }
        
        private byte? _minMemorySize;
        public byte? MinMemorySize
        {
            get => _minMemorySize;
            set
            {
                if (value <= 0)
                {
                    _minMemorySize = null;
                }

                _minMemorySize = value;
            }
        }

        public string Server { get; set; }

        private string _serverPort;
        public string ServerPort
        {
            get => _serverPort;
            set => _serverPort = value ?? "25565";
        }
        
        private int? _windowHeight;
        public int? WindowHeight
        {
            get => _windowHeight;
            set
            {
                if (value <= 0)
                {
                    _windowHeight = null;
                }

                _windowHeight = value;
            }
        }
        
        private int? _windowWidth;
        public int? WindowWidth
        {
            get => _windowWidth;
            set
            {
                if (value <= 0)
                {
                    _windowWidth = null;
                }

                _windowWidth = value;
            }
        }

        public bool? Fullscreen { get; set; }

        public string JvmArgs { get; set; }

        public Launcher(MinecraftLocator locator = null, string launcherName = null, byte? maxMemorySize = default, byte? minMemorySize = default, string serverPort = null, int? windowHeight = default, int? windowWidth = default, AuthenticateResult auth = null, string server = null, bool? fullscreen = default, string jvmArgs = null, string javaPath = null)
        {
            Locator = locator;
            LauncherName = launcherName;
            MaxMemorySize = maxMemorySize;
            MinMemorySize = minMemorySize;
            ServerPort = serverPort;
            WindowHeight = windowHeight;
            WindowWidth = windowWidth;
            Auth = auth;
            Server = server;
            Fullscreen = fullscreen;
            JvmArgs = jvmArgs;
            JavaPath = javaPath;
        }

        public string GetArgument(string version, bool isolation = true)
        {
            var mc = Locator.GetMinecraft(version, isolation);
            var argument = new StringBuilder();

            #region Jvm args

            argument.Append($"{JvmArgs} ");
            argument.Append("-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump ");
            argument.Append("-XX:+UnlockExperimentalVMOptions ");
            argument.Append("-XX:+UseG1GC ");
            argument.Append("-XX:G1NewSizePercent=20 ");
            argument.Append("-XX:G1ReservePercent=20 ");
            argument.Append("-XX:MaxGCPauseMillis=50 ");
            argument.Append("-XX:G1HeapRegionSize=32M ");
            argument.Append($"-Djava.library.path=\"{mc.File.Natives}\" ");
            argument.Append($"-Xmx{MaxMemorySize}G ");
            argument.Append(MinMemorySize != null ? $"-Xmn{MaxMemorySize}G " : string.Empty);
            argument.Append($"-classpath \"{string.Join(';', Locator.GetLibraries(version))};");
            argument.Append(!mc.File.Jar.Exists ? $"{mc.Inherit.File.Jar}\" " : $"{mc.File.Jar}\" ");

            #endregion

            #region Main class

            argument.Append($"{mc.Json.MainClass} ");

            #endregion

            #region Minecraft args

            argument.Append($"--username \"{Auth.Name}\" ");
            argument.Append($"--uuid \"{Auth.Uuid}\" ");
            argument.Append($"--accessToken \"{Auth.AccessToken}\" ");
            argument.Append($"--assetsDir \"{mc.File.Assets}\" ");
            argument.Append($"--assetIndex \"{mc.RootVersion}\" ");
            argument.Append($"--gameDir \"{mc.File.Root}\" ");
            argument.Append($"--versionType \"{LauncherName}\" ");
            argument.Append($"--version \"{LauncherName}\" ");
            argument.Append(WindowWidth != null ? $"--width {WindowWidth} " : string.Empty);
            argument.Append(WindowHeight != null ? $"--height {WindowHeight} " : string.Empty);
            argument.Append(Fullscreen != null ? $"--fullscreen " : string.Empty);
            argument.Append("--userProperties {} ");

            #endregion

            #region Attachment

            if (mc.Type.IsLoader())
            {
                var all = string.Empty;
                if (mc.Type == Minecraft.MinecraftJson.MinecraftType.NewLoader)
                {
                    mc.Json.Arguments["game"].ToObject<JArray>().ForEach(x =>
                    {
                        if (x is JValue)
                        {
                            all += $"{x} ";
                        }
                    });
                }
                else
                {
                    all = mc.Json.MinecraftArguments;
                }

                if (!string.IsNullOrEmpty(all))
                    argument.Append($"{all.Substring(all.LastIndexOf("} ") + 2)}");
            }
            argument.Append(mc.Type.IsLoader() ? $"" : string.Empty);

            #endregion

            return argument.ToString().Trim();
        }

        public Process Launch(string version, bool isolation = true)
        {
            ExtractNatives(version, isolation);
            
            var mc = Locator.GetMinecraft(version, isolation);
            
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = JavaPath,
                    Arguments = GetArgument(version),
                    WorkingDirectory = mc.File.Root.FullName,
                    UseShellExecute = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            return process;
        }

        public void ExtractNatives(string version, bool isolation = true)
        {
            var mc = Locator.GetMinecraft(version, isolation);
            mc.File.Natives.Create();
            
            var natives = Locator.GetNatives(version);
            natives.ForEach(x =>
            {
                ZipFile.OpenRead(x.File.FullName).Entries.ForEach(z =>
                {
                    if (z.FullName.EndsWith("dll"))
                    {
                        z.ExtractToFile($@"{mc.File.Natives}\{z.FullName}", true);
                    }
                });
            });
        }
    }
}