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

        public virtual async Task<AuthenticateResult> Authenticate()
        {
            var payload = this.GetPayload(AuthenticateEndpoints.Authenticate);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Authenticate, payload);
            var json = JObject.Parse(response.Content);
            
            return json.GetAuthenticateResult(response.StatusCode == HttpStatusCode.OK);
        }

        public virtual async Task<AuthenticateResult> Refresh(AuthenticateResult result)
        {
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Refresh, payload);
            var json = JObject.Parse(response.Content);

            return json.GetAuthenticateResult(response.StatusCode == HttpStatusCode.OK);
        }

        public virtual async Task<bool> Validate(AuthenticateResult result)
        {
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Validate, payload);
            
            return response.StatusCode == HttpStatusCode.NoContent;
        }
        
        public virtual async Task<bool> Validate(string accessToken)
        {
            var payload = this.GetPayload(accessToken);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Validate, payload);
            
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public virtual async Task Invalidate(AuthenticateResult result)
        {
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            await HttpUtils.Post(AuthenticateEndpoints.Invalidate, payload);
        }

        public virtual async Task Signout()
        {
            var payload = this.GetPayload(AuthenticateEndpoints.Signout);
            await HttpUtils.Post(AuthenticateEndpoints.Signout, payload);
        }
    }
}