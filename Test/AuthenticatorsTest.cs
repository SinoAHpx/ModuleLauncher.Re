using System.Threading.Tasks;
using AHpx.ModuleLauncher.Authenticators;
using AHpx.ModuleLauncher.Data.Authentications;
using Xunit;

namespace Test
{
    public class AuthenticatorsTest
    {
        private const string Username = "AHpx@yandex.com";
        private const string Password = "asd123,./";
        private const string ClientToken = "2933170747";
        private readonly OnlineAuthenticator _authenticator = new OnlineAuthenticator(Username, Password, ClientToken);
        private AuthenticateResult _result;
        
        [Fact(Skip = "passed")]
        public async Task AuthTest()
        {
            _result = await _authenticator.Authenticate();

            var expect = "AHpxChina";
            var actual = _result.Name;

            Assert.Equal(expect, actual);
            Assert.True(_result.Verified);

            var acc = _result.AccessToken;
            Assert.True(await _authenticator.Validate(_result));
            
            _result = await _authenticator.Refresh(_result);
            Assert.True(await _authenticator.Validate(_result));
            Assert.False(await _authenticator.Validate(acc));

            await _authenticator.Invalidate(_result);
            Assert.False(await _authenticator.Validate(_result));
            
            _result = await _authenticator.Authenticate();
            await _authenticator.Signout();
            Assert.False(await _authenticator.Validate(_result));
        }

        [Fact(Skip = "passed")]
        public async Task ExternalAuth()
        {
            var expect = "yushijinhun's demo auth server";
            var actual = (await new ExternalAuthenticator("https://auth-demo.yushi.moe", "", "").GetMetadata()).Meta
                .ServerName;
            
            Assert.Equal(expect, actual);
        }
    }
}