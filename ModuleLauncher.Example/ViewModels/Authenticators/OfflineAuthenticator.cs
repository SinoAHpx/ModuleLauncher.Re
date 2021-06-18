using System;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Models.Authenticators;

namespace ModuleLauncher.Example.ViewModels.Authenticators
{
    public class OfflineAuthenticator : AuthenticatorBase
    {
        /// <summary>
        /// Your offline account name
        /// </summary>
        public override string Account { get; set; }
        
        [Obsolete("You don't have to set password for a offline authenticator")]
        public override string Password { get; set; }

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