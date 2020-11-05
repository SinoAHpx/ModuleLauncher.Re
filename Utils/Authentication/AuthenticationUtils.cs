using System;
using AHpx.ModuleLauncher.Authenticators;
using AHpx.ModuleLauncher.Data.Authentications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Utils.Authentication
{
    public static class AuthenticationUtils
    {
        public static string GetValue(this AuthenticateEndpoints ex)
        {
            return ex.ToString().ToLower();
        }

        public static string GetPayload(this OnlineAuthenticator authenticator, AuthenticateEndpoints endpoints)
        {
            switch (endpoints)
            {
                case AuthenticateEndpoints.Authenticate:
                    return GetAuthenticatePayload(authenticator);
                case AuthenticateEndpoints.Signout:
                    return GetSignoutPayload(authenticator);
                case AuthenticateEndpoints.Refresh:
                case AuthenticateEndpoints.Validate:
                case AuthenticateEndpoints.Invalidate:
                    throw new ArgumentOutOfRangeException(nameof(endpoints), endpoints, null);
                default:
                    throw new ArgumentOutOfRangeException(nameof(endpoints), endpoints, null);
            }
        }

        public static string GetPayload(this OnlineAuthenticator authenticator, string accessToken, string clientToken)
        {
            return GetTokenPayload(accessToken, clientToken);
        }
        
        public static string GetPayload(this OnlineAuthenticator authenticator, string accessToken)
        {
            return GetTokenPayload(accessToken);
        }

        private static string GetAuthenticatePayload(OnlineAuthenticator authenticator)
        {
            var obj = new
            {
                agent = new
                {
                    name = "minecraft",
                    version = 1
                },
                username = authenticator.Username,
                password = authenticator.Password,
                clientToken = authenticator.ClientToken
            };

            return JsonConvert.SerializeObject(obj);
        }

        private static string GetSignoutPayload(OnlineAuthenticator authenticator)
        {
            var obj = new
            {
                username = authenticator.Username,
                password = authenticator.Password
            };

            return JsonConvert.SerializeObject(obj);
        }
        
        private static string GetTokenPayload(string accessToken, string clientToken)
        {
            var obj = new
            {
                accessToken,
                clientToken
            };
            
            return JsonConvert.SerializeObject(obj);
        }
        
        private static string GetTokenPayload(string accessToken)
        {
            var obj = new
            {
                accessToken
            };
            
            return JsonConvert.SerializeObject(obj);
        }

        internal static AuthenticateResult GetAuthenticateResult(this JObject json, bool condition)
        {
            return condition
                ? new AuthenticateResult
                {
                    AccessToken = json["accessToken"]?.ToString(),
                    ClientToken = json["clientToken"]?.ToString(),
                    Name = json["selectedProfile"]?["name"]?.ToString(),
                    Uuid = json["selectedProfile"]?["id"]?.ToString(),
                    Verified = true
                }
                : new AuthenticateResult
                {
                    Error = json["error"]?.ToString(),
                    ErrorMessage = json["errorMessage"]?.ToString(),
                    Verified = false
                };
        }
    }
}