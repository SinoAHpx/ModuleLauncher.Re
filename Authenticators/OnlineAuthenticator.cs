using System.Net;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Authentication;
using AHpx.ModuleLauncher.Utils.Authentication;
using AHpx.ModuleLauncher.Utils.Network;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Authenticators
{
    public class OnlineAuthenticator
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientToken { get; set; }

        public OnlineAuthenticator(string username = null, string password = null, string clientToken = null)
        {
            Username = username;
            Password = password;
            ClientToken = clientToken;
        }

        public OnlineAuthenticator(string username = null, string password = null)
        {
            Username = username;
            Password = password;
        }

        public async Task<AuthenticateResult> Authenticate()
        {
            var payload = this.GetPayload(AuthenticateEndpoints.Authenticate);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Authenticate, payload);
            var json = JObject.Parse(response.Content);
            
            return response.StatusCode == HttpStatusCode.OK
                ? new AuthenticateResult
                {
                    AccessToken = json["accessToken"]?.ToString(),
                    ClientToken = json["clientToken"]?.ToString(),
                    Name = json["selectedProfile"]?["name"]?.ToString(),
                    Uuid = json["selectedProfile"]?["id"]?.ToString(),
                    Verified = true
                }
                : new AuthenticateResult
                {
                    Error = json["error"]?.ToString(),
                    ErrorMessage = json["errorMessage"]?.ToString(),
                    Verified = false
                };
        }
    }
}