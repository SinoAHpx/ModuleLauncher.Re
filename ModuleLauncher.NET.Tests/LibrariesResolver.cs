using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Tests;

public class LibrariesResolver
{
    [Fact]
    public void TestRawNameParser()
    {
        var rawName1 = "com.mojang:logging:1.0.0".ResolveRawName();
        rawName1.ShouldBe("com/mojang/logging/1.0.0/logging-1.0.0.jar");

        var rawName2 = "net.java.dev.jna:jna-platform:5.10.0".ResolveRawName();
        rawName2.ShouldBe("net/java/dev/jna/jna-platform/5.10.0/jna-platform-5.10.0.jar");

        var rawName3 = "commons-codec:commons-codec:1.15".ResolveRawName('\\');
        rawName3.ShouldBe("commons-codec\\commons-codec\\1.15\\commons-codec-1.15.jar");
    }
}