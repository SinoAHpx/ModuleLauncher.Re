using System;
using System.Net;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Authenticators
{
    [Obsolete("This authentication method will be abandoned in late 2021")]
    public class MojangAuthenticator : AuthenticatorBase
    {
        private string _account;
        private string _password;
        private string _clientToken;

        /// <summary>
        /// Mojang account for authenticate, can't be null
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public override string Account
        {
            get => _account ?? throw new ArgumentNullException($"{nameof(Account)} can't be null!");
            set => _account = value ?? throw new ArgumentNullException($"{nameof(Account)} can't be null!");
        }

        /// <summary>
        /// The password of the Mojang account, can't be null
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public override string Password
        {
            get => _password ?? throw new ArgumentNullException($"{nameof(Password)} can't be null!");
            set => _password = value ?? throw new ArgumentNullException($"{nameof(Password)} can't be null!");
        }

        /// <summary>
        /// Client identifier, if its value is null, a new Guid will be instead
        /// </summary>
        public override string ClientToken
        {
            get => _clientToken ?? Guid.NewGuid().ToString("N");
            set => _clientToken = value ?? Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Default constructor with optional parameters
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
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

        /// <summary>
        /// Execute authenticate via account and password given
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw GetException(result);
            }

            return result.Content.ToJsonEntity<AuthenticateResult>();
        }

        /// <summary>
        /// Refreshes a valid accessToken. It can be used to keep a user logged in between gaming sessions and is preferred over storing the user's password in a file
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="clientToken">If its value is null, the ClientToken property of current object will be use</param>
        /// <returns></returns>
        public override async Task<AuthenticateResult> Refresh(string accessToken, string clientToken = null)
        {
            if (clientToken.IsNullOrEmpty())
            {
                clientToken = ClientToken;
            }

            var url = "https://authserver.mojang.com/refresh";
            var payload = new
            {
                accessToken,
                clientToken,
                requestUser = true
            }.ToJsonString();

            var result = await HttpUtility.PostJson(url, payload);
            
            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw GetException(result);
            }

            return result.Content.ToJsonEntity<AuthenticateResult>(); 
        }

        /// <summary>
        /// Checks if an accessToken is usable for authentication with a Minecraft server.
        /// The Minecraft Launcher (as of version 1.6.13) calls this endpoint on startup to verify that its saved token is still usable,
        /// and calls Refesh if this returns an error.
        /// </summary>
        /// <param name="accessToken">Note that an accessToken may be unusable for authentication with a Minecraft server,
        /// but still be good enough for Refresh.
        /// This mainly happens when one has used another client (e.g. played Minecraft on another PC with the same account).
        /// It seems only the most recently obtained accessToken for a given account can reliably be used for authentication (the next-to-last token also seems to remain valid, but don't rely on it).</param>
        /// <param name="clientToken">Validate may be called with or without a clientToken. If a clientToken is provided,
        /// it should match the one used to obtain the accessToken.
        /// The Minecraft Launcher does send a clientToken to Validate</param>
        /// <param name="throwException">Throw exception when validate result is false</param>
        /// <returns></returns>
        public override async Task<bool> Validate(string accessToken, string clientToken = null, bool throwException = false)
        {
            var url = "https://authserver.mojang.com/validate";
            var payload = new
            {
                accessToken,
                clientToken
            }.ToJsonString();

            var result = await HttpUtility.PostJson(url, payload);
            
            if (throwException)
            {
                if (result.StatusCode != HttpStatusCode.NoContent)
                {
                    throw GetException(result);
                }
            }

            return result.StatusCode == HttpStatusCode.NoContent;
        }
    }
}