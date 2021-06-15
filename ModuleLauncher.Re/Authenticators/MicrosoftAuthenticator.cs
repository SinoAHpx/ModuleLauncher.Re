using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ModuleLauncher.Re.Authenticators
{
    public class MicrosoftAuthenticator : AuthenticatorBase
    {
        [Obsolete("Please use Code property instead")]
        public override string Account
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        
        [Obsolete("Please use Code property instead")]
        public override string Password
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string Code { get; set; }

        public async Task<bool> CheckMinecraftOwnership(string accessToken)
        {
            var result = await HttpUtility.Get("https://api.minecraftservices.com/entitlements/mcstore",
                new Dictionary<string, string>(new[]
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {accessToken}")
                }));

            return JArray.Parse(JObject.Parse(result.Content).Fetch("items")).Count() != 0;
        }
        
        public override async Task<AuthenticateResult> Authenticate()
        {
            //We can get a authorize code from webview: https://login.live.com/oauth20_authorize.srf?client_id=00000000402b5328 &response_type=code &scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL &redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf
            
            //Authorize code -> authorize token: post https://login.live.com/oauth20_token.srf

            var rest = new RestClient("https://login.live.com/oauth20_token.srf");
            rest.AddDefaultQueryParameter("client_id", "00000000402b5328");
            rest.AddDefaultQueryParameter("code", Code);
            rest.AddDefaultQueryParameter("grant_type", "authorization_code");
            rest.AddDefaultQueryParameter("redirect_uri", "https://login.live.com/oauth20_desktop.srf");
            rest.AddDefaultQueryParameter("scope", "service::user.auth.xboxlive.com::MBI_SSL");

            rest.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded");
            
            // var msAuthorizePayload = new
            // {
            //     client_id = "00000000402b5328",
            //     code = Code,
            //     grant_type = "authorization_code",
            //     redirect_uri = "https://login.live.com/oauth20_desktop.srf",
            //     scope = "service::user.auth.xboxlive.com::MBI_SSL"
            // }.ToJsonString();

            //Execute the post, fetch access_token from response json
            var msAuthorizeToken = JObject.Parse((await rest.ExecuteGetAsync(new RestRequest())).Content);

            //xbox live authenticate: post https://user.auth.xboxlive.com/user/authenticate
            var xboxLiveAuthenticatePayload = new
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = msAuthorizeToken
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            }.ToJsonString();

            //Execute the post, fetch Token and uhs in JArray
            var xboxLiveJObj = JObject.Parse((await HttpUtility.PostJson("https://user.auth.xboxlive.com/user/authenticate",
                xboxLiveAuthenticatePayload)).Content);
            var xboxLiveToken = xboxLiveJObj.Fetch("Token");
            var xboxLiveUhs = JArray.Parse(xboxLiveJObj.Fetch("xui")).First().Fetch("uhs");

            //XSTS authorize: post https://xsts.auth.xboxlive.com/xsts/authorize
            var xstsAuthorizePayload = new
            {
                Properties = new
                {
                    SandboxId = "RETAIL",
                    UserTokens = new[]
                    {
                        xboxLiveToken
                    }
                },
                RelyingParty = "rp://api.minecraftservices.com/",
                TokenType = "JWT"
            }.ToJsonString();

            //Execute post, fetch Token
            var xstsToken =
                JObject.Parse((await HttpUtility.PostJson("https://xsts.auth.xboxlive.com/xsts/authorize",
                    xstsAuthorizePayload)).Content).Fetch("Token");
            
            //Minecraft authenticate: post https://api.minecraftservices.com/authentication/login_with_xbox
            var minecraftAuthenticatePayload = new
            {
                identityToken = $"XBL3.0 x={xboxLiveUhs};{xstsToken}"
            }.ToJsonString();

            //Execute post, fetch access_token
            var minecraftAccessToken = JObject.Parse((await HttpUtility.PostJson(
                    "https://api.minecraftservices.com/authentication/login_with_xbox", minecraftAuthenticatePayload))
                .Content).Fetch("access_token");
            
            //Fetch out profile, so we can return a AuthenticateResult object

            var minecraftProfileJObj = JObject.Parse((await HttpUtility.Get(
                "https://api.minecraftservices.com/minecraft/profile",
                new Dictionary<string, string>(new[]
                {
                    new KeyValuePair<string, string>("Authorization", $"Bearer {minecraftAccessToken}")
                }))).Content);

            if (!await CheckMinecraftOwnership(minecraftAccessToken))
            {
                throw new Exception("Your account has no Minecraft");
            }
            
            return new AuthenticateResult
            {
                Uuid = minecraftProfileJObj.Fetch("id"),
                Name = minecraftProfileJObj.Fetch("name"),
                AccessToken = minecraftAccessToken,
                ClientToken = null,
            };
        }

        public override Task<AuthenticateResult> Refresh(string accessToken, string clientToken = null)
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> Validate(string accessToken, string clientToken = null, bool throwException = false)
        {
            throw new System.NotImplementedException();
        }

        public override Task SignOut()
        {
            throw new System.NotImplementedException();
        }

        public override Task Invalidate(string accessToken, string clientToken)
        {
            throw new System.NotImplementedException();
        }
    }
}