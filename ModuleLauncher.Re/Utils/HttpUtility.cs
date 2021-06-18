using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModuleLauncher.Re.Utils.Extensions;
using RestSharp;

namespace ModuleLauncher.Re.Utils
{
    /// <summary>
    /// Http related tools
    /// </summary>
    public static class HttpUtility
    {
        private static RestClient _client;
        
        static HttpUtility()
        {
            _client = new RestClient();
        }

        /// <summary>
        /// Post json content to specify url with custom content type(optional)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<IRestResponse> PostJson(string url, string json, string contentType = null)
        {
            _client.BaseUrl = new Uri(url);

            var request = new RestRequest(Method.POST);
            request.AddJsonBody(json);

            if (!contentType.IsNullOrEmpty())
            {
                request.AddHeader("Content-Type", contentType!);
            }

            return await _client.ExecuteAsync(request);
        }
        
        /// <summary>
        /// Send a get request to specify url with a custom header dictionary(optinal)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="customHeaders"></param>
        /// <returns></returns>
        public static async Task<IRestResponse> Get(string url, Dictionary<string, string> customHeaders = null)
        {
            _client.BaseUrl = new Uri(url);

            var request = new RestRequest(Method.GET);

            if (customHeaders != null)
            {
                request.AddHeaders(customHeaders);
            }

            return await _client.ExecuteAsync(request);
        }
        
        /// <summary>
        /// Send a get request to specify url with a custom content type(optional)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static async Task<IRestResponse> Get(string url, string contentType)
        {
            _client.BaseUrl = new Uri(url);

            var request = new RestRequest(Method.GET);

            if (!contentType.IsNullOrEmpty())
            {
                request.AddHeader("Content-Type", contentType);
            }

            return await _client.ExecuteAsync(request);
        }
    }
}