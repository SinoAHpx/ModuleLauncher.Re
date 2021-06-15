using System;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Authenticators;

namespace ModuleLauncher.Re.Authenticators
{
    public class MicrosoftAuthenticator : AuthenticatorBase
    {
        [Obsolete("Please use Code property instead")]
        public override string Account
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        
        [Obsolete("Please use Code property instead")]
        public override string Password
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string Code { get; set; }

        public override Task<AuthenticateResult> Authenticate()
        {
            throw new System.NotImplementedException();

        }

        public override Task<AuthenticateResult> Refresh(string accessToken, string clientToken = null)
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> Validate(string accessToken, string clientToken = null, bool throwException = false)
        {
            throw new System.NotImplementedException();
        }

        public override Task SignOut()
        {
            throw new System.NotImplementedException();
        }

        public override Task Invalidate(string accessToken, string clientToken)
        {
            throw new System.NotImplementedException();
        }
    }
}