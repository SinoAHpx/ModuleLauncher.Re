using System;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Authenticators
{
    public class MojangAuthenticator : AuthenticatorBase
    {
        private string _account;
        private string _password;
        private string _clientToken;

        public override string Account
        {
            get => _account ?? throw new ArgumentNullException($"{nameof(Account)} can't be null!");
            set => _account = value ?? throw new ArgumentNullException($"{nameof(Account)} can't be null!");
        }

        public override string Password
        {
            get => _password ?? throw new ArgumentNullException($"{nameof(Password)} can't be null!");
            set => _password = value ?? throw new ArgumentNullException($"{nameof(Password)} can't be null!");
        }

        public override string ClientToken
        {
            get => _clientToken ?? Guid.NewGuid().ToString("N");
            set => _clientToken = value ?? Guid.NewGuid().ToString("N");
        }

        public MojangAuthenticator()
        {
            ClientToken = null;
        }

        public MojangAuthenticator(string account = null, string password = null)
        {
            _account = account;
            _password = password;
        }

        public MojangAuthenticator(string account, string password, string clientToken = null)
        {
            _account = account;
            _password = password;
            _clientToken = clientToken;
        }

        public override async Task<AuthenticateResult> Authenticate()
        {
            var url = "https://authserver.mojang.com/authenticate";
            var payload = new
            {
                agent = new
                {
                    name = "Minecraft",
                    version = 1
                },
                username = Account,
                password = Password,
                clientToken = ClientToken,
                requestUser = true
            }.ToJsonString();

            var result = await HttpUtility.PostJson(url, payload);

            if (!IsSuccess(result))
            {
                throw GetException(result);
            }

            return result.Content.ToJsonEntity<AuthenticateResult>();
        }
    }
}