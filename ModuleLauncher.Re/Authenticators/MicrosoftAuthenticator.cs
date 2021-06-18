using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ModuleLauncher.Re.Authenticators
{
    /// <summary>
    /// MicrosoftAuthenticator, there's a only Authenticate and CheckMinecraftOwnership method available
    /// </summary>
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

        public MicrosoftAuthenticator(string code = null)
        {
            Code = code;
        }

        public async Task<bool> CheckMinecraftOwnership(string accessToken)
        {
            var result = await HttpUtility.Get("https://api.minecraftservices.com/entitlements/mcstore",
                new Dictionary<string, string>
                {
                    {"Authorization", $"Bearer {accessToken}"}
                });

            return JArray.Parse(JObject.Parse(result.Content).Fetch("items")).Count() != 0;
        }
        
        /// <summary>
        /// Execute Microsoft authentication via authorize code
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticateResult> Authenticate()
        {
            //We can get authorize code from the web browser by visit:
            //https://login.live.com/oauth20_authorize.srf?client_id=00000000402b5328&response_type=code&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf

            var microsoftAuthorizeToken = await GetMicrosoftAuthorizeToken();

            var xboxLiveTokenAndUhs = await GetXboxLiveTokenAndUhs(microsoftAuthorizeToken);
            var xstsToken = await GetXSTSToken(xboxLiveTokenAndUhs["token"]);
            var minecraftToken =
                await GetMinecraftAccessToken(xboxLiveTokenAndUhs["uhs"], xstsToken);

            var re = await GetMinecraftProfile(minecraftToken);

            return re;
        }
        
        private async Task<string> GetMicrosoftAuthorizeToken()
        {
            var url = new StringBuilder("https://login.live.com/oauth20_token.srf");
            url.Append("?client_id=00000000402b5328&");
            url.Append($"code={Code}&");
            url.Append("grant_type=authorization_code&");
            url.Append("redirect_uri=https://login.live.com/oauth20_desktop.srf&");
            url.Append("scope=service::user.auth.xboxlive.com::MBI_SSL&");

            var response = await HttpUtility.Get(url.ToString(), "Content-Type: application/x-www-form-urlencoded");
            var json = response.Content.ToJObject();

            try
            {
                var token = json.Fetch("access_token");
                
                return token;
            }
            catch (Exception e)
            {
                throw new Exception($"Error: {json.Fetch("error")}\r\nMessage: {json.Fetch("error_description")}", e);
            }
        }
        
        private async Task<Dictionary<string, string>> GetXboxLiveTokenAndUhs(string microsoftAuthorizeToken)
        {
            var url = "https://user.auth.xboxlive.com/user/authenticate";
            var payload = new
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = microsoftAuthorizeToken
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            }.ToJsonString();

            var response = await HttpUtility.PostJson(url, payload);
            var json = response.Content.ToJObject();

            var re = new Dictionary<string, string> {{"token", json.Fetch("Token")}};

            var jArr = json["DisplayClaims"].Fetch("xui").ToJArray();

            re.Add("uhs", jArr.First.Fetch("uhs"));
            
            return re;
        }

        private async Task<string> GetXSTSToken(string xboxLiveToken)
        {
            var url = "https://xsts.auth.xboxlive.com/xsts/authorize";
            var payload = new
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

            var response = await HttpUtility.PostJson(url, payload);
            var json = response.Content.ToJObject();
            
            return json.Fetch("Token");
        }

        private async Task<string> GetMinecraftAccessToken(string uhs, string xstsToken)
        {
            var url = "https://api.minecraftservices.com/authentication/login_with_xbox";
            var payload = new
            {
                identityToken = $"XBL3.0 x={uhs};{xstsToken}"
            }.ToJsonString();

            var response = await HttpUtility.PostJson(url, payload);
            var json = response.Content.ToJObject();

            return json.Fetch("access_token");
        }

        private async Task<AuthenticateResult> GetMinecraftProfile(string accessToken)
        {
            var url = "https://api.minecraftservices.com/minecraft/profile";
            var response = await HttpUtility.Get(url, new Dictionary<string, string>
            {
                {"Authorization", $"Bearer {accessToken}"}
            });

            var json = response.Content.ToJObject();

            if (json.ContainsKey("error"))
            {
                throw new Exception($"This account does not have minecraft ownership");
            }

            var re = new AuthenticateResult
            {
                Name = json.Fetch("name"),
                Uuid = json.Fetch("id"),
                AccessToken = accessToken,
                ClientToken = null
            };

            return re;
        }
    }
}