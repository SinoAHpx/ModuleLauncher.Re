using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Authenticators
{
    public abstract class AuthenticatorBase
    {
        public abstract string Account { get; set; }
        public abstract string Password { get; set; }
        public virtual string ClientToken { get; set; }
        
        public abstract Task<AuthenticateResult> Authenticate();

        public virtual Task<AuthenticateResult> Refresh(string accessToken, string clientToken = null) => null;

        public virtual Task<bool> Validate(string accessToken, string clientToken = null, bool throwException = false) => null;

        public virtual Task SignOut() => null;

        public virtual Task Invalidate(string accessToken, string clientToken) => null;

        internal Exception GetException(string response)
        {
            var obj = JObject.Parse(response);
                
            return new Exception($"Error: {obj.Fetch("error")}\r\nMessage: {obj.Fetch("errorMessage")}");
        }
    }
}