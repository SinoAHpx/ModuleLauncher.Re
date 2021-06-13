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
        public string Account { get; set; }
        public string Password { get; set; }
        public string ClientToken { get; set; }
        
        public abstract Task<AuthenticateResult> Authenticate();

        internal Exception GetException(IRestResponse response)
        {
            var obj = JObject.Parse(response.Content);
                
            return new Exception($"Error: {obj.Fetch("error")}\r\nMessage: {obj.Fetch("errorMessage")}");
        }

        internal bool IsSuccess(IRestResponse response)
        {
            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}