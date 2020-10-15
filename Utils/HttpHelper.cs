using System;
using System.Threading.Tasks;
using ModuleLauncher.Re.DataEntities.Utils;
using ModuleLauncher.Re.Extensions;
using RestSharp;

namespace ModuleLauncher.Re.Utils
{
    //head
    public partial class HttpHelper
    {
        private const string UserAgent = "ModuleLauncher.Re/2.5";
    }

    //sync
    public partial class HttpHelper
    {
        public static HttpResult GetHttp(string uri)
        {
            return GetHttpAsync(uri).GetResult();
        }

        public static HttpResult PostHttp(string uri, string json)
        {
            return PostHttpAsync(uri, json).GetResult();
        }
    }

    //async
    public partial class HttpHelper
    {
        public static async Task<HttpResult> GetHttpAsync(string uri)
        {
            var result = await new RestClient
            {
                BaseUrl = new Uri(uri),
                UserAgent = UserAgent
            }.ExecuteAsync(new RestRequest
            {
                Method = Method.GET
            });

            return new HttpResult
            {
                Content = result.Content,
                StatusCode = result.StatusCode
            };
        }

        public static async Task<HttpResult> PostHttpAsync(string uri, string json)
        {
            var client = new RestClient
            {
                BaseUrl = new Uri(uri),
                UserAgent = UserAgent
            };
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(json);

            var result = await client.ExecuteAsync(request);
            return new HttpResult
            {
                Content = result.Content,
                StatusCode = result.StatusCode
            };
        }
    }
}