using System;
using System.Net;
using System.Threading.Tasks;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Models.Authenticators;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Utils.Extensions
{
    public static class MojangAuthenticatorExtensions
    {
        private static MojangAuthenticator _mojangAuthenticator = new MojangAuthenticator();
        
        /// <summary>
        /// Invalidate a access token from MojangAuthenticator
        /// </summary>
        /// <param name="result"></param>
        public static async Task Invalidate(this AuthenticateResult result)
        {
            await _mojangAuthenticator.Invalidate(result.AccessToken, result.ClientToken);
        }

        /// <summary>
        /// Validate a access token from MojangAuthenticator
        /// </summary>
        /// <param name="authenticateResult"></param>
        /// <param name="throwException">Throw when exception occured</param>
        public static async Task<bool> Validate(this AuthenticateResult authenticateResult, bool throwException = false)
        {
            return await _mojangAuthenticator.Validate(authenticateResult.AccessToken, authenticateResult.ClientToken,
                throwException);
        }
        
        /// <summary>
        /// Refresh a access token from MojangAuthenticator
        /// </summary>
        /// <param name="authenticateResult"></param>
        public static async Task<AuthenticateResult> Refresh(this AuthenticateResult authenticateResult)
        {
            return await _mojangAuthenticator.Refresh(authenticateResult.AccessToken, authenticateResult.ClientToken);
        }
    }
}