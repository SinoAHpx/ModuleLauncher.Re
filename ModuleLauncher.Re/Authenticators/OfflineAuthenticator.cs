using System;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Authenticators;

namespace ModuleLauncher.Re.Authenticators
{
    public class OfflineAuthenticator : AuthenticatorBase
    {
        /// <summary>
        /// Your offline account name
        /// </summary>
        public string Account { get; set; }

        public OfflineAuthenticator(string account = null)
        {
            Account = account;
        }

        public override Task<AuthenticateResult> Authenticate()
        {
            return Task.Run(() => new AuthenticateResult
            {
                Name = Account,
                AccessToken = Guid.NewGuid().ToString("N"),
                ClientToken = Guid.NewGuid().ToString("N"),
                Uuid = Guid.NewGuid().ToString("N"),
            });
        }
    }
}