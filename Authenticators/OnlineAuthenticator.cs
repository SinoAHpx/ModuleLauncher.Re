using System;
using System.Net;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Authentications;
using AHpx.ModuleLauncher.Utils.Authentication;
using AHpx.ModuleLauncher.Utils.Extensions;
using AHpx.ModuleLauncher.Utils.Network;
using Newtonsoft.Json.Linq;

namespace AHpx.ModuleLauncher.Authenticators
{
    /// <summary>
    /// 参考自https://wiki.vg/Authentication
    /// </summary>
    public class OnlineAuthenticator
    {
        public string Username { get; set; }
        public string Password { get; set; }
        
        /// <summary>
        /// ClientToken should be a randomly generated identifier and must be the same every time the request is
        /// </summary>
        public string ClientToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="clientToken">ClientToken should be a randomly generated identifier and must be the same every time the request is</param>
        public OnlineAuthenticator(string username = null, string password = null, string clientToken = null)
        {
            Username = username;
            Password = password;
            ClientToken = clientToken;
        }

        public OnlineAuthenticator(string username = null, string password = null)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Authenicate users using password
        /// </summary>
        /// <returns></returns>
        public virtual async Task<AuthenticateResult> Authenticate()
        {
            if (Username.IsNullOrEmpty() || Password.IsNullOrEmpty())
            {
                throw new ArgumentException($"Invalid {nameof(Username)} or {nameof(Password)}!");
            }
            
            var payload = this.GetPayload(AuthenticateEndpoints.Authenticate);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Authenticate, payload);
            var json = JObject.Parse(response.Content);
            
            return json.GetAuthenticateResult(response.StatusCode == HttpStatusCode.OK);
        }

        /// <summary>
        /// Refresh a valid AccessToken. It can be used to keep login in the game session
        /// </summary>
        /// <param name="result">Note: The AccessToken provided will be invalid.</param>
        /// <returns></returns>
        public virtual async Task<AuthenticateResult> Refresh(AuthenticateResult result)
        {
            if (result == null)
            {
                throw new ArgumentException($"Argument {nameof(result)} can't be null!");
            }
            
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Refresh, payload);
            var json = JObject.Parse(response.Content);

            return json.GetAuthenticateResult(response.StatusCode == HttpStatusCode.OK);
        }

        /// <summary>
        /// Check if AccessToken can be used for authentication of the Minecraft server.
        /// </summary>
        /// <param name="result">Can be called when there is or without ClientToken. If you provide ClientToken, it should match the one get access to AccessToken</param>
        /// <returns></returns>
        public virtual async Task<bool> Validate(AuthenticateResult result)
        {
            if (result == null)
            {
                throw new ArgumentException($"Argument {nameof(result)} can't be null!");
            }
            
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Validate, payload);
            
            return response.StatusCode == HttpStatusCode.NoContent;
        }
        
        /// <summary>
        /// Check if AccessToken can be used for authentication of the Minecraft server.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public virtual async Task<bool> Validate(string accessToken)
        {
            if (accessToken.IsNullOrEmpty())
            {
                throw new ArgumentException($"Argument {nameof(accessToken)} can't be null!");
            }
            
            var payload = this.GetPayload(accessToken);
            var response = await HttpUtils.Post(AuthenticateEndpoints.Validate, payload);
            
            return response.StatusCode == HttpStatusCode.NoContent;
        }

        /// <summary>
        /// Use the Client / Access token to make AccessToken fail.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual async Task Invalidate(AuthenticateResult result)
        {
            if (result == null)
            {
                throw new ArgumentException($"Argument {nameof(result)} can't be null!");
            }
            
            var payload = this.GetPayload(result.AccessToken, result.ClientToken);
            await HttpUtils.Post(AuthenticateEndpoints.Invalidate, payload);
        }

        /// <summary>
        /// Use an account username and password to make AccessToken useless.
        /// </summary>
        /// <returns></returns>
        public virtual async Task Signout()
        {
            if (Username.IsNullOrEmpty() || Password.IsNullOrEmpty())
            {
                throw new ArgumentException($"Invalid {nameof(Username)} or {nameof(Password)}!");
            }
            
            var payload = this.GetPayload(AuthenticateEndpoints.Signout);
            await HttpUtils.Post(AuthenticateEndpoints.Signout, payload);
        }
    }
}