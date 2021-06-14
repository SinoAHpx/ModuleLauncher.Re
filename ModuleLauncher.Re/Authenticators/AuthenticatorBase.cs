using System;
using System.Net;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ModuleLauncher.Re.Authenticators
{
    public abstract class AuthenticatorBase
    {
        public abstract string Account { get; set; }
        public abstract string Password { get; set; }
        public virtual string ClientToken { get; set; }
        
        public abstract Task<AuthenticateResult> Authenticate();

        public abstract Task<AuthenticateResult> Refresh(string accessToken, string clientToken = null);

        public abstract Task<bool> Validate(string accessToken, string clientToken = null, bool throwException = false);

        public abstract Task SignOut();

        public abstract Task Invalidate(string accessToken, string clientToken);

        internal Exception GetException(IRestResponse response)
        {
            var obj = JObject.Parse(response.Content);
                
            return new Exception($"Error: {obj.Fetch("error")}\r\nMessage: {obj.Fetch("errorMessage")}");
        }
    }
}