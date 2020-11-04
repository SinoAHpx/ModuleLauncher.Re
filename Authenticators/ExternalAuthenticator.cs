using System.Net;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Authentication;
using AHpx.ModuleLauncher.Utils.Authentication;
using AHpx.ModuleLauncher.Utils.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Authenticators
{
    //TODO: This class has not been tested, there may be exceptions, if any, please submit an issue
    /// <summary>
    ///     Support authlib-injector, 
    ///     This class has not been tested, there may be exceptions, if any, please submit an issue, 
    ///     View authlib-injector document: https://github.com/yushijinhun/authlib-injector/wiki/%E5%90%AF%E5%8A%A8%E5%99%A8%E6%8A%80%E6%9C%AF%E8%A7%84%E8%8C%83
    /// </summary>
    public class ExternalAuthenticator : OnlineAuthenticator
    {
        public ExternalAuthenticator(string api = null, string username = null, string password = null,
            string clientToken = null) : base(username, password, clientToken)
        {
            Api = api;
        }

        public ExternalAuthenticator(string api = null, string username = null, string password = null)
            : base(username, password)
        {
            Api = api;
        }

        private string _api;
        public string Api
        {
            get => _api; 
            set => _api = value.TrimEnd('/');
        }

        public async Task<ExternalAuthenticateMeta> GetMetadata()
        {
            return JsonConvert.DeserializeObject<ExternalAuthenticateMeta>((await HttpUtils.Get(Api)).Content);
        }

        public override async Task<AuthenticateResult> Authenticate()
        {
            var payload = this.GetPayload(AuthenticateEndpoints.Authenticate);
            var response = await HttpUtils.Post($"{Api}/authserver/{AuthenticateEndpoints.Authenticate.GetValue()}",
                payload);
            var json = JObject.Parse(response.Content);
            
            return json.GetAuthenticateResult(response.StatusCode == HttpStatusCode.OK);
        }

        public override async Task<AuthenticateResult> Refresh(AuthenticateResult result)
        {
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            var response = await HttpUtils.Post($"{Api}/authserver/{AuthenticateEndpoints.Refresh.GetValue()}", payload);
            var json = JObject.Parse(response.Content);

            return json.GetAuthenticateResult(response.StatusCode == HttpStatusCode.OK);
        }

        public override async Task<bool> Validate(AuthenticateResult result)
        {
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            var response = await HttpUtils.Post($"{Api}/authserver/{AuthenticateEndpoints.Validate.GetValue()}", payload);
            
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public override async Task<bool> Validate(string accessToken)
        {
            var payload = this.GetPayload(accessToken);
            var response = await HttpUtils.Post($"{Api}/authserver/{AuthenticateEndpoints.Validate.GetValue()}", payload);
            
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public override async Task Invalidate(AuthenticateResult result)
        {
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            await HttpUtils.Post($"{Api}/authserver/{AuthenticateEndpoints.Invalidate.GetValue()}", payload);
        }

        public override async Task Signout()
        {
            var payload = this.GetPayload(AuthenticateEndpoints.Signout);
            await HttpUtils.Post($"{Api}/authserver/{AuthenticateEndpoints.Signout.GetValue()}", payload);
        }
    }
}