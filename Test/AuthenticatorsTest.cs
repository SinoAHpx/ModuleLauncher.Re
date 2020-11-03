using System.Threading.Tasks;
using AHpx.ModuleLauncher.Authenticators;
using Xunit;

namespace Test
{
    public class AuthenticatorsTest
    {
        [Fact]
        public async Task AuthTest()
        {
            var a = new OnlineAuthenticator("ahpx@yandex.com", "asd123,./");
            var r = await a.Authenticate();

            var expect = "AHpxChina";
            var actual = r.Name;

            Assert.Equal(expect, actual);
            Assert.True(r.Verified);
        }
    }
}