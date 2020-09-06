using System.Net;

namespace ModuleLauncher.Re.Utils.DataEntity.Utils
{
    public class HttpResult
    {
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}