using System;
using AHpx.ModuleLauncher.Data.Authentication;

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
    }
}