using System.Net;
using System.Threading.Tasks;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.DataEntity.Authenticator;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Authenticator
{
    //head
    public partial class YggdrasilAuthenticator
    {
        /// <summary>
        /// 用于认证的mojang账户，可以是电子邮箱地址或玩家名称（对于未迁移的账号）
        /// </summary>
        public string Username
        {
            get => _payload.Username;
            set => _payload.Username = value;
        }

        /// <summary>
        /// 用于认证的mojang密码
        /// </summary>
        public string Password
        {
            get => _payload.Password;
            set => _payload.Password = value;
        }

        /// <summary>
        /// clientToken应该是一个随机生成的标识符而且必须每次请求都是相同的。
        /// </summary>
        public string ClientToken
        {
            get => _payload.ClientToken;
            set => _payload.ClientToken = value;
        }

        private readonly AuthenticatorHelper _payload;

        public YggdrasilAuthenticator(string username = "", string password = "", string clientToken = "")
        {
            _payload = new AuthenticatorHelper
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
            var payload = _payload.GetAuthenticatePayload();
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
            var payload = _payload.GetRefreshPayload(accessToken);
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
            var payload = _payload.GetValidatePayload(accessToken);
            var result = await HttpHelper.PostHttpAsync(ValidateDomain, payload);

            return result.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// 使用client/access令牌对使accessToken失效。
        /// </summary>
        /// <param name="accessToken">有效的accessToken</param>
        /// <param name="clientToken">这需要第一处用来获取</param>
        /// <returns></returns>
        public async Task InvalidateAsync(string accessToken, string clientToken)
        {
            var payload = _payload.GetInvalidatePayload(accessToken, clientToken);
            await HttpHelper.PostHttpAsync(InvalidateDomain, payload);
        }

        /// <summary>
        /// 使用帐号的用户名和密码使accessToken失效。
        /// </summary>
        /// <returns></returns>
        public async Task SignOutAsync()
        {
            var payload = _payload.GetSignOutPayload();
            await HttpHelper.PostHttpAsync(SignOutDomain, payload);
        }
    }

    //sync
    public partial class YggdrasilAuthenticator
    {
        /// <summary>
        /// 使用密码认证用户。
        /// </summary>
        /// <returns></returns>
        public AuthenticateResult Authenticate()
        {
            return AuthenticateAsync().GetResult();
        }

        /// <summary>
        /// 刷新一个有效的accessToken。它可以用于在游戏会话间保持登录状态，这优于在文件中保存用户的密码（见lastlogin）。
        /// </summary>
        /// <param name="accessToken">注意：提供的accessToken将失效。</param>
        /// <returns></returns>
        public AuthenticateResult Refresh(string accessToken)
        {
            return RefreshAsync(accessToken).GetResult();
        }

        /// <summary>
        /// 检查accessToken是否可用于Minecraft服务器的认证。
        /// </summary>
        /// <param name="accessToken">有效的accessToken</param>
        /// <returns></returns>
        public bool Validate(string accessToken)
        {
            return ValidateAsync(accessToken).GetResult();
        }

        /// <summary>
        /// 使用client/access令牌对使accessToken失效。
        /// </summary>
        /// <param name="accessToken">有效的accessToken</param>
        /// <param name="clientToken">这需要第一处用来获取</param>
        /// <returns></returns>
        public void Invalidate(string accessToken, string clientToken)
        {
            InvalidateAsync(accessToken, clientToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 使用帐号的用户名和密码使accessToken失效。
        /// </summary>
        /// <returns></returns>
        public void SignOut()
        {
            SignOutAsync().GetAwaiter().GetResult();
        }
    }
}