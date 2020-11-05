using System;
using System.IO;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using Xunit;

namespace Test
{
    public class LocatorsTest
    {
        private MinecraftLocator _location = new MinecraftLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft");
        
        [Theory]
        [InlineData("1.8",false)]
        [InlineData("1.5.2",false)]
        [InlineData("1.8.9-forge1.8.9-11.15.1.2318-1.8.9",false)]
        [InlineData("1.16.3-OptiFine_HD_U_G3",false)]
        [InlineData("fabric-loader-0.10.3+build.211-1.15.2",false)]
        [InlineData("Hyperium 1.8.9",false)]
        public void t1(string name, bool isolation)
        {
            // Assert.All(new MinecraftLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft").GetMinecrafts(true), (minecraft =>
            // {
            //     Assert.True(minecraft.File.Json.FullName.IsJson());
            //     Assert.True(minecraft.File.Jar == null || minecraft.File.Jar.Extension.Contains("jar"));
            //     Assert.Equal(@"C:\Users\ahpx\AppData\Roaming\.minecraft\libraries", minecraft.File.Libraries.FullName);
            //     Assert.Equal(@"C:\Users\ahpx\AppData\Roaming\.minecraft\assets", minecraft.File.Assets.FullName);
            //     Assert.Equal(minecraft.File.Version.FullName + "\\mods", minecraft.File.Mod.FullName);
            // }));

            Assert.Equal($@"C:\Users\ahpx\AppData\Roaming\.minecraft\versions\{name}\{name}.json",
                _location.GetMinecraft(name, isolation).File.Json.FullName);

            var ac = _location.GetMinecraft(name, isolation).File.Mod.FullName;
            Assert.Equal(@"C:\Users\ahpx\AppData\Roaming\.minecraft\mods", ac);
        }

        [Theory]
        //[InlineData("org.apache.commons:commons-lang3:3.1")]
        // [InlineData("commons-io:commons-io:2.4")]
        // [InlineData("net.java.jinput:jinput:2.0.5")]
        [InlineData("org.lwjgl.lwjgl:lwjgl_util:2.9.0")]
        public void t2(string name)
        {
            var ex = "org/lwjgl/lwjgl/lwjgl_util/2.9.0/lwjgl_util-2.9.0.jar".Replace("/", "\\");
            var ac = name.ToLibraryFile();

            Assert.Equal(ex, ac);
        }
    }
}