using System.Net;

namespace AHpx.ModuleLauncher.Data.Utils
{
    public class HttpResponse
    {
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}