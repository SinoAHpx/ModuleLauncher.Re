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
        public abstract Task<AuthenticateResult> Authenticate();

        internal Exception GetException(string response)
        {
            var obj = JObject.Parse(response);
                
            return new Exception($"Error: {obj.Fetch("error")}\r\nMessage: {obj.Fetch("errorMessage")}");
        }
    }
}