using System.Net;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Authentications;
using AHpx.ModuleLauncher.Utils.Authentication;
using AHpx.ModuleLauncher.Utils.Network;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Authenticators
{
    /// <summary>
    /// 参考自https://wiki.vg/Authentication
    /// </summary>
    public class OnlineAuthenticator
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        /// <summary>
        /// clientToken应该是一个随机生成的标识符而且必须每次请求都是相同的
        /// </summary>
        public string ClientToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="clientToken">clientToken应该是一个随机生成的标识符而且必须每次请求都是相同的</param>
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

        /// <summary>
        /// 使用密码认证用户
        /// </summary>
        /// <returns></returns>
        public virtual async Task<AuthenticateResult> Authenticate()
        {
            var payload = this.GetPayload(AuthenticateEndpoints.Authenticate);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Authenticate, payload);
            var json = JObject.Parse(response.Content);
            
            return json.GetAuthenticateResult(response.StatusCode == HttpStatusCode.OK);
        }

        /// <summary>
        /// 刷新一个有效的accessToken。它可以用于在游戏会话间保持登录状态
        /// </summary>
        /// <param name="result">注意：提供的accessToken将失效。</param>
        /// <returns></returns>
        public virtual async Task<AuthenticateResult> Refresh(AuthenticateResult result)
        {
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Refresh, payload);
            var json = JObject.Parse(response.Content);

            return json.GetAuthenticateResult(response.StatusCode == HttpStatusCode.OK);
        }

        /// <summary>
        /// 检查accessToken是否可用于Minecraft服务器的认证。
        /// </summary>
        /// <param name="result">可以在有或没有clientToken时调用。如果提供了clientToken，它应当与获取accessToken的那个相匹配</param>
        /// <returns></returns>
        public virtual async Task<bool> Validate(AuthenticateResult result)
        {
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Validate, payload);
            
            return response.StatusCode == HttpStatusCode.NoContent;
        }
        
        /// <summary>
        /// 检查accessToken是否可用于Minecraft服务器的认证。
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public virtual async Task<bool> Validate(string accessToken)
        {
            var payload = this.GetPayload(accessToken);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Validate, payload);
            
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// 使用client/access令牌对使accessToken失效。
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual async Task Invalidate(AuthenticateResult result)
        {
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            await HttpUtils.Post(AuthenticateEndpoints.Invalidate, payload);
        }

        /// <summary>
        /// 使用帐号的用户名和密码使accessToken失效。
        /// </summary>
        /// <returns></returns>
        public virtual async Task Signout()
        {
            var payload = this.GetPayload(AuthenticateEndpoints.Signout);
            await HttpUtils.Post(AuthenticateEndpoints.Signout, payload);
        }
    }
}