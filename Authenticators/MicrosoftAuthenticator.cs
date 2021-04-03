using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Authentications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AHpx.ModuleLauncher.Authenticators
{
    public class MicrosoftAuthenticator
    {
        public MicrosoftAuthenticator(string microsoftAuthorizationCode = null)
        {
            MicrosoftAuthorizationCode = microsoftAuthorizationCode;
        }

        public string MicrosoftAuthorizationCode { get; set; }

        /// <summary>
        /// Authenticate microsoft account by using authorization code that can be obtain in web
        /// </summary>
        /// <returns></returns>
        public async Task<AuthenticateResult> Authenticate()
        {
            var re = new AuthenticateResult
            {
                ClientToken = null
            };

            try
            {
                var microsoftAuthorizationToken = await GetMSToken(MicrosoftAuthorizationCode);
                var xblResult = await AuthenticateXbl(microsoftAuthorizationToken);
                var xstsToken = await AuthenticateXsts(xblResult);

                var minecraftAccessToken = await AuthenticateMinecraft(xblResult, xstsToken);
                re.AccessToken = minecraftAccessToken;

                var profileJsonObj = JObject.Parse(await GetMcProfile(minecraftAccessToken));
                re.Uuid = profileJsonObj["id"].ToString();
                re.Name = profileJsonObj["name"].ToString();
                re.Verified = true;
            }
            catch (Exception ex)
            {
                re.Error = ex.ToString();
                re.ErrorMessage = ex.Message;
                re.Verified = false;
            }

            return re;
        }

        private async Task<string> GetMSToken(string MScode)
        {
            var rest = new RestClient("https://login.live.com/oauth20_token.srf");
            rest.AddDefaultQueryParameter("client_id", "00000000402b5328");
            rest.AddDefaultQueryParameter("code", MScode);
            rest.AddDefaultQueryParameter("grant_type", "authorization_code");
            rest.AddDefaultQueryParameter("redirect_uri", "https://login.live.com/oauth20_desktop.srf");
            rest.AddDefaultQueryParameter("scope", "service::user.auth.xboxlive.com::MBI_SSL");

            rest.AddDefaultHeader("Content-Type", "application/x-www-form-urlencoded");

            var res = await rest.ExecuteGetAsync(new RestRequest());
            
            return JObject.Parse(res.Content)["access_token"]?.ToString();
        }
        
        private async Task<Dictionary<string, string>> AuthenticateXbl(string MSToken)
        {
            //https://user.auth.xboxlive.com/user/authenticate
            var re = new Dictionary<string, string>();

            var payload = JsonConvert.SerializeObject(new
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = MSToken
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            });

            var rest = new RestClient("https://user.auth.xboxlive.com/user/authenticate");
            var req = new RestRequest();
            req.AddJsonBody(payload);

            var res = (await rest.ExecutePostAsync(req)).Content;

            var json = JObject.Parse(res);

            re.Add("token", json["Token"].ToString());

            re.Add("uhs", JArray.Parse(json["DisplayClaims"]["xui"].ToString()).First["uhs"].ToString());

            return re;
        }
        
        private async Task<string> AuthenticateXsts(Dictionary<string, string> XBLresult)
        {
            //https://xsts.auth.xboxlive.com/xsts/authorize

            var payload = JsonConvert.SerializeObject(new
            {
                Properties = new
                {
                    SandboxId = "RETAIL",
                    UserTokens = new[] { XBLresult["token"] }
                },
                RelyingParty = "rp://api.minecraftservices.com/",
                TokenType = "JWT"
            });

            var rest = new RestClient("https://xsts.auth.xboxlive.com/xsts/authorize");
            var req = new RestRequest();
            req.AddJsonBody(payload);

            var res = (await rest.ExecutePostAsync(req)).Content;
            
            return JObject.Parse(res)["Token"].ToString();
        }
        
        private async Task<string> AuthenticateMinecraft(Dictionary<string, string> XBLresult, string XSTStoken)
        {
            //https://api.minecraftservices.com/authentication/login_with_xbox

            var payload = JsonConvert.SerializeObject(new
            {
                identityToken = $"XBL3.0 x={XBLresult["uhs"]};{XSTStoken}"
            });

            var rest = new RestClient("https://api.minecraftservices.com/authentication/login_with_xbox");
            var req = new RestRequest();
            req.AddJsonBody(payload);

            var res = (await rest.ExecutePostAsync(req)).Content;

            return JObject.Parse(res)["access_token"].ToString();
        }
        
        /// <summary>
        /// Check minecraft ownership by using minecraft access token
        /// </summary>
        /// <param name="MCtoken">Minecraft access token</param>
        /// <returns></returns>
        public async Task<bool> CheckMinecraftOwnership(string MCtoken)
        {
            //https://api.minecraftservices.com/entitlements/mcstore

            var rest = new RestClient("https://api.minecraftservices.com/entitlements/mcstore");
            rest.AddDefaultHeader("Authorization", $"Bearer {MCtoken}");

            var res = (await rest.ExecuteGetAsync(new RestRequest())).Content;

            var jsonArr = JArray.Parse(JObject.Parse(res)["items"].ToString());

            return jsonArr.Count > 0;
        }
        
        private async Task<string> GetMcProfile(string MCtoken)
        {
            //https://api.minecraftservices.com/minecraft/profile
            var rest = new RestClient("https://api.minecraftservices.com/minecraft/profile");
            rest.AddDefaultHeader("Authorization", $"Bearer {MCtoken}");

            var res = (await rest.ExecuteGetAsync(new RestRequest())).Content;

            return res;
        }
    }
}