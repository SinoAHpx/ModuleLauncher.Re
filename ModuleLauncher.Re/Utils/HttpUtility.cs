using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ModuleLauncher.Re.Utils.Extensions;

namespace ModuleLauncher.Re.Utils
{
    /// <summary>
    /// Http related tools
    /// </summary>
    internal static class HttpUtility
    {
        /// <summary>
        /// Post json content to specify url with custom content type(optional)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> PostJson(string url, string json, string contentType = "application/json")
        {
            var client = new HttpClient();
            var content = new StringContent(json, Encoding.Default, contentType);
            
            var response = await client.PostAsync(url, content);

            return response;
        }

        /// <summary>
        /// Send a get request to specify url with a authorization header
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authorization"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> Get(string url, string authorization = null, string scheme = "Bearer")
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, authorization);
            
            var response = await client.GetAsync(url);

            return response;
        }
        
        /// <summary>
        /// Send a get request to specify url with a custom content type(optional)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        internal static async Task<HttpResponseMessage> Get(string url, string contentType)
        {
            var client = new HttpClient();
      
            var response = await client.GetAsync(url);

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            
            return response;
        }
    }
}