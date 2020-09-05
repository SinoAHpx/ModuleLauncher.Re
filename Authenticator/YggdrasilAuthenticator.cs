using System.Net;
using System.Threading.Tasks;
using ModuleLauncher.Re.Service;
using ModuleLauncher.Re.Service.DataEntity.Authenticator;
using ModuleLauncher.Re.Service.Extensions;
using ModuleLauncher.Re.Service.Utils;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Authenticator
{
    //head
    public partial class YggdrasilAuthenticator
    {
        public string Username
        {
            get => Payload.Username;
            set => Payload.Username = value;
        }

        public string Password
        {
            get => Payload.Password;
            set => Payload.Password = value;
        }
        public string ClientToken 
        { 
            get => Payload.ClientToken;
            set => Payload.ClientToken = value;
        }

        private AuthenticatorPayload Payload;
        public YggdrasilAuthenticator(string username = "", string password = "", string clientToken = "")
        {
            Payload = new AuthenticatorPayload
            {
                Username = username,
                Password = password,
                ClientToken = clientToken
            };
        }

        private const string AuthDomain = "https://authserver.mojang.com/authenticate";
        private const string RefreshDomain = "https://authserver.mojang.com/refresh";
        private const string ValidateDomain = "https://authserver.mojang.com/validate";
        private const string InvalidateDomain = "https://authserver.mojang.com/invalidate";
        private const string SignOutDomain = "https://authserver.mojang.com/signout";
    }
    
    //async
    public partial class YggdrasilAuthenticator
    {
        /// <summary>
        /// 使用密码认证用户。
        /// </summary>
        /// <returns></returns>
        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            var payload = Payload.GetAuthenticatePayload();
            var result = await HttpHelper.PostHttpAsync(AuthDomain, payload);
            var response = JObject.Parse(result.Content);

            return result.StatusCode == HttpStatusCode.OK
                ? new AuthenticateResult
                {
                    AccessToken = response["accessToken"]?.ToString(),
                    ClientToken = response["clientToken"]?.ToString(),
                    Username = response["selectedProfile"]?["name"]?.ToString(),
                    Uuid = response["selectedProfile"]?["id"]?.ToString(),
                    Verified = true
                }
                : new AuthenticateResult
                {
                    Error = response["error"]?.ToString(),
                    ErrorMessage = response["errorMessage"]?.ToString(),
                    Verified = false
                };
        }
        
        /// <summary>
        /// 刷新一个有效的accessToken。它可以用于在游戏会话间保持登录状态，这优于在文件中保存用户的密码（见lastlogin）。
        /// </summary>
        /// <param name="accessToken">注意：提供的accessToken将失效。</param>
        /// <returns></returns>
        public async Task<AuthenticateResult> RefreshAsync(string accessToken)
        {
            var payload = Payload.GetRefreshPayload(accessToken);
            var result = await HttpHelper.PostHttpAsync(RefreshDomain, payload);
            var response = JObject.Parse(result.Content);

            return result.StatusCode == HttpStatusCode.OK
                ? new AuthenticateResult
                {
                    AccessToken = response["accessToken"]?.ToString(),
                    ClientToken = response["clientToken"]?.ToString(),
                    Username = response["selectedProfile"]?["name"]?.ToString(),
                    Uuid = response["selectedProfile"]?["id"]?.ToString(),
                    Verified = true
                }
                : new AuthenticateResult
                {
                    Error = response["error"]?.ToString(),
                    ErrorMessage = response["errorMessage"]?.ToString(),
                    Verified = false
                };
        }

        /// <summary>
        /// 检查accessToken是否可用于Minecraft服务器的认证。
        /// </summary>
        /// <param name="accessToken">有效的accessToken</param>
        /// <returns></returns>
        public async Task<bool> ValidateAsync(string accessToken)
        {
            var payload = Payload.GetValidatePayload(accessToken);
            var result = await HttpHelper.PostHttpAsync(ValidateDomain, payload);

            return result.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// 使用client/access令牌对使accessToken失效。
        /// </summary>
        /// <param name="accessToken">有效的accessToken</param>
        /// <param name="clientToken">这需要第一处用来获取</param>
        /// <returns></returns>
        public async Task InvalidateAsync(string accessToken,string clientToken)
        {
            var payload = Payload.GetInvalidatePayload(accessToken, clientToken);
            await HttpHelper.PostHttpAsync(InvalidateDomain, payload);
        }

        public async Task SignOutAsync()
        {
            var payload = Payload.GetSignOutPayload();
            await HttpHelper.PostHttpAsync(SignOutDomain, payload);
        }
    }
    
    //sync
    public partial class YggdrasilAuthenticator
    {
        public AuthenticateResult Authenticate() => AuthenticateAsync().GetResult();
        public AuthenticateResult Refresh(string accessToken) => RefreshAsync(accessToken).GetResult();
        public bool Validate(string accessToken) => ValidateAsync(accessToken).GetResult();
        public void Invalidate(string accessToken, string clientToken) =>
            InvalidateAsync(accessToken, clientToken).GetAwaiter().GetResult();
        public void SignOut() => SignOutAsync().GetAwaiter().GetResult();
    }
}