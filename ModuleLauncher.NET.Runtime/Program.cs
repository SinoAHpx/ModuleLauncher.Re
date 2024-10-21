using System.Runtime.InteropServices;
using Downloader;
using Flurl.Http;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Launcher;
using ModuleLauncher.NET.Models.Authentication;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Runtime;
using ModuleLauncher.NET.Utilities;
using Timer = System.Timers.Timer;

class Program
{
    public async static Task Main()
    {
        //https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.1-nightly-20130708-debug3/lwjgl-platform-2.9.1-nightly-20130708-debug3.jar
        //https://libraries.minecraft.net/org/lwjgl/lwjgl/lwjgl-platform/2.9.1-nightly-20130708-debug3/lwjgl-platform-2.9.1-nightly-20130708-debug3-natives-osx.jar"
         var resolver = new MinecraftResolver("/Volumes/Neo/Furnace/minecraft");
         var version = "1.6.4";
         var minecraft = await resolver.GetRemoteMinecraftAndToLocalAsync(version);
         if (!minecraft.ValidateChecksum())
         {
             Console.WriteLine($"{minecraft} is not exist, downloading");
             await minecraft.GetDownloadUrl().DownloadFileAsync(minecraft.Tree.VersionRoot.FullName, minecraft.Tree.Jar.Name);
             Console.WriteLine($"Download completed");
         }
         
         var libraries = minecraft.GetLibraries().Where(l => !l.ValidateChecksum()).ToList();
         if (libraries.Count != 0)
         {
             Console.WriteLine($"Have {libraries.Count} libraries to download");
             foreach (var libraryEntry in libraries)
             {
                 Console.WriteLine($"Downloading {libraryEntry.File.Name}");
                 await libraryEntry.GetDownloadUrl().DownloadFileAsync(libraryEntry.File.Directory.FullName, libraryEntry.File.Name);
             }

             Console.WriteLine("Download completed");
         }

         var assets = (await minecraft.GetAssetsAsync()).Where(a => !a.ValidateChecksum()).ToList();
         if (assets.Count != 0)
         {
             Console.WriteLine($"Have {assets.Count} assets to download");
             foreach (var assetEntry in assets)
             {
                 Console.WriteLine($"Downloading {assetEntry.File.Name}");
                 await assetEntry.GetDownloadUrl().DownloadFileAsync(assetEntry.File.Directory.FullName, assetEntry.File.Name);
             }

             Console.WriteLine("Download completed");
         }

         var launcher = new Launcher
         {
             LauncherConfig = new LauncherConfig
             {
                 Javas =
                 [
                     new MinecraftJava
                     {
                         Executable = new FileInfo("/Library/Java/JavaVirtualMachines/temurin-21.jdk/Contents/Home/bin/java"),
                         Version = 21
                     },
                     new MinecraftJava
                     {
                         Executable = new FileInfo("/Library/Java/JavaVirtualMachines/temurin-8.jdk/Contents/Home/bin/jar"),
                         Version = 8
                     }
                 ],
                 Authentication = "AHpx"
             }
         };
         launcher.GetLaunchArguments(minecraft).Print();

         var process = await launcher.LaunchAsync(minecraft); 

         while (await process.ReadOutputLineAsync() is {} output)
         {
             Console.WriteLine(output);
         }
    }
}

static class RuntimeUtils
{

    public static T Print<T>(this T t)
    {
        Console.WriteLine(t);

        return t;
    }
}