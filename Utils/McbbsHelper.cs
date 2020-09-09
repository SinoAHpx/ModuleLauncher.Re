using System.Threading.Tasks;
using HtmlAgilityPack;
using ModuleLauncher.Re.Extensions;

namespace ModuleLauncher.Re.Utils
{
    internal class McbbsHelper
    {
        private const string Forum = "https://www.mcbbs.net/portal.php";
        
        internal static HtmlNode GetNode() => GetNodeAsync().GetResult();
        
        internal static async Task<HtmlNode> GetNodeAsync()
        {
            var node = new HtmlDocument();
            node.LoadHtml(await GetHtmlAsync());

            return node.DocumentNode;
        }
        
        private static async Task<string> GetHtmlAsync()
        {
            return (await HttpHelper.GetHttpAsync(Forum)).Content;
        } 
    }
}