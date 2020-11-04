using System;
using System.IO;
using AHpx.ModuleLauncher.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;
using Xunit;

namespace Test
{
    public class LocatorsTest
    {
        [Fact]
        public void t1()
        {
            Assert.All(new MinecraftLocator(@"C:\Users\ahpx\AppData\Roaming\.minecraft").GetMinecrafts(), (minecraft =>
            {
                Assert.True(minecraft.File.Json.FullName.IsJson());
                Assert.True(minecraft.File.Jar == null || minecraft.File.Jar.Extension.Contains("jar"));
            }));
        }
    }
}