using System;
using System.Threading.Tasks;
using RestSharp;

namespace ModuleLauncher.Re.Utils
{
    public static class HttpUtility
    {
        private static RestClient _client;
        
        static HttpUtility()
        {
            _client = new RestClient();
        }

        public static async Task<IRestResponse> PostJson(string url, string json)
        {
            _client.BaseUrl = new Uri(url);

            var request = new RestRequest(Method.POST);
            request.AddJsonBody(json);

            return await _client.ExecuteAsync(request);
        }
    }
}