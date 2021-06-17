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
            
        public override async Task<AuthenticateResult> Authenticate()
        {
            //We can get authorize code from the web browser by visit:
            //https://login.live.com/oauth20_authorize.srf?client_id=00000000402b5328&response_type=code&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf

            
            
            return null;
        }

        public async Task<string> GetMicrosoftAuthorizeToken()
        {
            var url = new StringBuilder("https://login.live.com/oauth20_token.srf");
            url.Append("?client_id=00000000402b5328&");
            url.Append($"code={Code}&");
            url.Append("grant_type=authorization_code&");
            url.Append("redirect_uri=https://login.live.com/oauth20_desktop.srf&");
            url.Append("scope=service::user.auth.xboxlive.com::MBI_SSL&");

            var response = await HttpUtility.Get(url.ToString(), "Content-Type: application/x-www-form-urlencoded");
            var jObj = response.Content.ToJObject();

            try
            {
                var token = jObj.Fetch("access_token");
                
                return token;
            }
            catch (Exception e)
            {
                throw new Exception($"Error: {jObj.Fetch("error")}\r\nMessage: {jObj.Fetch("error_description")}", e);
            }
        }

        public async Task<Dictionary<string, string>> AuthenticateXboxLive(string token)
        {
            var url = "https://user.auth.xboxlive.com/user/authenticate";
            var payload = new
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = token
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            }.ToJsonString();

            var response = await HttpUtility.PostJson(url, payload);
            var jObj = response.Content.ToJObject();

            var re = new Dictionary<string, string> {{"token", jObj.Fetch("Token")}};

            var jArr = jObj["DisplayClaims"].Fetch("xui").ToJArray();

            re.Add("uhs", jArr.First.Fetch("uhs"));
            
            return re;
        }
    }
}