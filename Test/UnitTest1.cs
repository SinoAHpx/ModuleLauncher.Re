using System;
using System.IO;
using System.Net;
using System.Reflection;
using AHpx.ModuleLauncher.Data.Authentication;
using AHpx.ModuleLauncher.Utils.Authentication;
using AHpx.ModuleLauncher.Utils.Network;
using Xunit;

namespace Test
{
    public class UnitTest1
    {
        [Fact(DisplayName = "GetTest", Skip = "Passed")]
        public async void Test1()
        {
            const HttpStatusCode expect = HttpStatusCode.OK;
            var actual = await HttpUtils.Get("https://v1.jinrishici.com/all.txt");
            
            Assert.Equal(expect, actual.StatusCode);
        }

        [Fact(DisplayName = "PostTest", Skip = "Passed")]
        public async void Test2()
        {
            const HttpStatusCode expect = HttpStatusCode.OK;
            var actual = await HttpUtils.Post("https://authserver.mojang.com/authenticate",
                await File.ReadAllTextAsync(@"defaultAuthJson.json"));
            
            Assert.Equal(expect, actual.StatusCode);
        }

        [Theory]
        [InlineData(AuthenticateEndpoints.Authenticate)]
        // [InlineData(AuthenticateEndpoints.Refresh)]
        // [InlineData(AuthenticateEndpoints.Validate)]
        // [InlineData(AuthenticateEndpoints.Invalidate)]
        // [InlineData(AuthenticateEndpoints.Signout)]
        public void Test3(AuthenticateEndpoints authenticateEndpoints)
        {
            var expect = "authenticate";
            var actual = authenticateEndpoints.GetValue();
            
            Assert.Equal(expect, actual);
        }
    }
    
}
