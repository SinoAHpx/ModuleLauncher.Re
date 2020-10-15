using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Masuit.Tools;
using ModuleLauncher.Re.Minecraft.Locator;

namespace ModuleLauncher.Re.Launcher
{
    public class LauncherCore
    {
        public LauncherArguments LauncherArguments { get; set; }
        public string JavaPath { get; set; }

        public void ExtractNatives(string name)
        {
            var locator = LauncherArguments.MinecraftLocator;
            var lib = new LibrariesLocator(locator);
            var ls = lib.GetNatives(name);
            var directory =
                new DirectoryInfo(
                    $"{LauncherArguments.MinecraftLocator.Location}\\versions\\{name}\\{name}-natives");

            if (directory.Exists)
                directory.GetFiles().ToList().ForEach(x => File.Delete(x.FullName));
            else directory.Create();

            ls.ForEach(x =>
            {
                if (!File.Exists(x.Path)) return;

                var archive = ZipFile.OpenRead(x.Path);
                archive.Entries.ToList().ForEach(z =>
                {
                    if (z.FullName.EndsWith("dll"))
                        z.ExtractToFile($"{locator.Location}\\versions\\{name}\\{name}-natives\\{z.FullName}",
                            true);
                });
            });
        }

        public Process Launch(string name, bool extractNatives = true)
        {
            if (extractNatives) ExtractNatives(name);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = JavaPath,
                    Arguments = LauncherArguments.GetArgument(name),
                    WorkingDirectory = LauncherArguments.MinecraftLocator.Location,
                    UseShellExecute = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                }
            };

            process.Start();
            return process;
        }
    }
}