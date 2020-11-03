using System;
using AHpx.ModuleLauncher.Authenticators;
using AHpx.ModuleLauncher.Data.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Utils.Authentication
{
    public static class AuthenticationUtils
    {
        public static string GetValue(this AuthenticateEndpoints ex)
        {
            // new feature but useless in here lol
            // return ex switch
            // {
            //     AuthenticateEndpoints.Authenticate => nameof(AuthenticateEndpoints.Authenticate).ToLower(),
            //     AuthenticateEndpoints.Refresh => nameof(AuthenticateEndpoints.Refresh).ToLower(),
            //     AuthenticateEndpoints.Validate => nameof(AuthenticateEndpoints.Validate).ToLower(),
            //     AuthenticateEndpoints.Invalidate => nameof(AuthenticateEndpoints.Invalidate).ToLower(),
            //     AuthenticateEndpoints.Signout => nameof(AuthenticateEndpoints.Signout).ToLower(),
            //     _ => throw new ArgumentOutOfRangeException(nameof(ex))
            // };

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

        public static string GetPayload(string accessToken, string clientToken)
        {
            return GetTokenPayload(accessToken, clientToken);
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
    }
}