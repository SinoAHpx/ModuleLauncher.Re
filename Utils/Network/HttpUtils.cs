using System;
using System.Reflection;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Authentication;
using AHpx.ModuleLauncher.Utils.Authentication;
using RestSharp;
using HttpResponse = AHpx.ModuleLauncher.Data.Utils.HttpResponse;

namespace AHpx.ModuleLauncher.Utils.Network
{
    public static class HttpUtils
    {
        public static readonly string UserAgent;

        static HttpUtils()
        {
            var ver = typeof(Entrance).Assembly.GetName().Version;
            UserAgent = $"{typeof(Entrance).Namespace?.Split('.')[1]}/{ver?.Major}.{ver?.Minor}";
        }

        public static async Task<HttpResponse> Get(string url)
        {
            return await Execute(url, new RestRequest(Method.GET));
        }
        
        public static async Task<HttpResponse> Post(string url, string json)
        {
            return await Execute(url, new RestRequest(Method.POST).AddJsonBody(json));
        }

        public static async Task<HttpResponse> Post(AuthenticateEndpoints endpoints, string json)
        {
            return await Post($"https://authserver.mojang.com/{endpoints.GetValue()}", json);
        }

        private static async Task<HttpResponse> Execute(string url, IRestRequest request)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(url),
                UserAgent = UserAgent
            };

            var result = await client.ExecuteAsync(request);
            return new HttpResponse()
            {
                Content = result.Content,
                StatusCode = result.StatusCode
            };
        }
    }
}