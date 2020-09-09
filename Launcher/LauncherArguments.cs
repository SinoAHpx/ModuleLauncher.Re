using System;
using System.IO;
using System.Text;
using Masuit.Tools;
using ModuleLauncher.Re.DataEntities.Authenticator;
using ModuleLauncher.Re.DataEntities.Enums;
using ModuleLauncher.Re.DataEntities.Launcher;
using ModuleLauncher.Re.Minecraft.Locator;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Launcher
{
    //head
    public partial class LauncherArguments
    {
        public MinecraftLocator MinecraftLocator { get; set; }
        public AuthenticateResult Authentication { get; set; }
        /// <summary>
        /// 注意加后缀 "G"或者"M"
        /// </summary>
        public string MaxMemorySize { get; set; }

        /// <summary>
        /// 可省略,注意加后缀 "G"或者"M"
        /// </summary>
        public string MinMemorySize { get; set; }
        public string LauncherName { get; set; }
        public string JvmArgument { get; set; }

        public ConnectionConfig ConnectionConfig { get; set; }
        public ResolutionConfig ResolutionConfig { get; set; }
    }

    public partial class LauncherArguments
    {
        public string GetArgument(string name)
        {
            return $"{GetForeArguments(name)} {GetBackArguments(name)}";
        }
    }
    
    public partial class LauncherArguments
    {
        private string GetBackArguments(string name)
        {
            var entity = MinecraftLocator.GetMinecraftJsonEntity(name);
            var auth = Authentication;
            var arguments = new StringBuilder($"{entity.mainClass} ");
            var type = MinecraftLocator.GetMinecraftJsonType(name);

            arguments.Append($"--username {auth.Username} ");
            arguments.Append($"--version {name} ");
            arguments.Append($"--gameDir {MinecraftLocator.Location} ");

            arguments.Append(MinecraftLocator.GetMinecraftVersionRoot(name) == "legacy"
                ? $"--assetsDir {MinecraftLocator.Location}\\assets\\virtual\\legacy "
                : $"--assetsDir {MinecraftLocator.Location}\\assets ");
            
            arguments.Append($"--assetIndex {MinecraftLocator.GetMinecraftVersionRoot(name)} ");
            arguments.Append($"--uuid {auth.Uuid} ");
            arguments.Append($"--accessToken {auth.AccessToken} ");
            arguments.Append("--userType mojang ");

            if (ConnectionConfig != null) 
                arguments.Append($"--server {ConnectionConfig.IpAddress} --port {ConnectionConfig.Port} ");
            if (ResolutionConfig != null)
            {
                arguments.Append($"--height {ResolutionConfig.WindowHeight} --width {ResolutionConfig.WindowWidth} ");
                if (ResolutionConfig.FullScreen != null) arguments.Append("--fullscreen ");
            }

            arguments.Append("--userProperties {} ");

            if (type == MinecraftJsonType.LoaderNew)
            {

                var array = JArray.Parse(entity.arguments.GetValue("game")?.ToString() ??
                                         throw new Exception("json文件损坏"));
                
                array?.ForEach(x => arguments.Append($"{x} "));
            }

            if (type == MinecraftJsonType.Loader || type == MinecraftJsonType.LoaderOld)
            {
                var s = entity.minecraftArguments;
                arguments.Append($"{s.Substring(s.LastIndexOf("}", StringComparison.Ordinal) + 3)} ");
            }
            
            return arguments.ToString();
        }
        
        private string GetForeArguments(string name)
        {
            var arguments = new StringBuilder($"{JvmArgument} ");
            arguments.Append(
                "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump ");
            arguments.Append($"-Djava.library.path={MinecraftLocator.Location}\\versions\\{name}\\{name}-natives ");
            arguments.Append($"-classpath {GetClasspath(name)} ");
            var minMemory = string.IsNullOrEmpty(MinMemorySize) ? string.Empty : $"-Xmn{MinMemorySize} ";
            arguments.Append($"-Xmx{MaxMemorySize} {minMemory}");
            arguments.Append("-XX:+UnlockExperimentalVMOptions ");
            arguments.Append("-XX:+UseG1GC ");
            arguments.Append("-XX:G1NewSizePercent=20 ");
            arguments.Append("-XX:G1ReservePercent=20 ");
            arguments.Append("-XX:MaxGCPauseMillis=50 ");
            arguments.Append("-XX:G1HeapRegionSize=32M ");

            return arguments.ToString().Trim();
        }
        
        private string GetClasspath(string name)
        {
            var arguments = new StringBuilder();
            var libraries = string.Empty;
            new LibrariesLocator(MinecraftLocator).GetLibraries(name).ForEach(x => libraries += $"{x.Path};");

            arguments.Append($"{libraries}");

            var type = MinecraftLocator.GetMinecraftJsonType(name);
            if (type == MinecraftJsonType.LoaderNew || type == MinecraftJsonType.Loader)
                if (!File.Exists($"{MinecraftLocator.Location}\\versions\\{name}\\{name}.jar"))
                {
                    var inherit = MinecraftLocator.GetInheritsMinecraftJsonEntity(name).id;
                    try
                    {
                        File.Copy($"{MinecraftLocator.Location}\\versions\\{inherit}\\{inherit}.jar",
                            $"{MinecraftLocator.Location}\\versions\\{name}\\{name}.jar");
                    }
                    catch
                    {
                        MinecraftLocator.GetMinecraftFileEntities().ForEach(x =>
                        {
                            var entity = MinecraftLocator.GetMinecraftJsonEntity(x.Name);
                            if (inherit.Equals(entity.id))
                                File.Copy($"{MinecraftLocator.Location}\\versions\\{x}\\{x}.jar",
                                    $"{MinecraftLocator.Location}\\versions\\{name}\\{name}.jar");
                            else
                                throw new FileNotFoundException("Not found inherits form jar.");
                        });
                    }
                }

            arguments.Append($"{MinecraftLocator.Location}\\versions\\{name}\\{name}.jar");
            return arguments.ToString();
        }
    }
}