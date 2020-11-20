using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Utils;
using HtmlAgilityPack;

namespace AHpx.ModuleLauncher.Utils.Network
{
    public partial class McbbsNews
    {
        private const string Forum = "https://www.mcbbs.net/portal.php";

        private static async Task<HtmlNode> GetNode()
        {
            var node = new HtmlDocument();
            node.LoadHtml((await HttpUtils.Get(Forum)).Content);

            return node.DocumentNode;
        }
        
        private static async Task<IEnumerable<News.CarouselItem>> GetCarousel(string id)
        {
            var xpath = $"//div[@id='{id}']/div[@class='slideshow_item']/div[@class='image']/a";
            var nodeAsync = await GetNode();

            return from node in nodeAsync.SelectNodes(xpath)
                let imgNode = node.SelectSingleNode("img")
                select new News.CarouselItem
                {
                    Link = $"{Forum}/{node.GetAttributeValue("href", "").TrimStart('/')}",
                    Title = node.GetAttributeValue("title", ""), Image = imgNode.GetAttributeValue("src", "")
                };
        }

    }
    
    public partial class McbbsNews
    {
        public static async Task<IEnumerable<News.RecommendedItem>> GetModeratorRecommended()
        {
            const string xpath = "//div[@id='portal_block_729_content']/div/div[@class='portal_li']";
            var node = await GetNode();
            var re = new List<News.RecommendedItem>();

            //找出版主推荐
            foreach (var selectNode in node.SelectNodes(xpath))
            {
                var moderatorRecommended = new News.RecommendedItem();
                //遍历portal_li元素的所有子元素
                foreach (var htmlNode in selectNode.SelectNodes("child::*"))
                    //遍历该子元素的所有attribute
                foreach (var attribute in htmlNode.Attributes)
                {
                    //如果该子元素有一个href属性，它的值就是要添加的链接
                    if (attribute.Name == "href")
                        moderatorRecommended.Link = $"{Forum}/{attribute.Value.TrimStart('/')}";

                    switch (attribute.Value)
                    {
                        //如果该子元素的attribute的值为portal_li_bottom则可以定位至portal_li_bottom这个div
                        case "portal_li_bottom":
                        {
                            foreach (var selectNode1 in htmlNode.SelectNodes("child::*"))
                                //这个div有两个div作为子元素，以class这个attribute的值作为区分
                                if (selectNode1.GetAttributeValue("class", "") == "portal_li_bl")
                                    moderatorRecommended.Author = selectNode1.InnerText;
                                else
                                    moderatorRecommended.CommentCount = selectNode1.InnerText;
                            break;
                        }
                        case "portal_li_img":
                        {
                            var imgNode = htmlNode.SelectSingleNode("img");
                            moderatorRecommended.Image = imgNode.GetAttributeValue("src", "").Trim();
                            moderatorRecommended.Title = imgNode.GetAttributeValue("title", "");
                            moderatorRecommended.DisplayTitle = imgNode.GetAttributeValue("alt", "");
                            break;
                        }
                    }
                }

                re.Add(moderatorRecommended);
            }

            return re;
        }
        
        public static async Task<IEnumerable<News.RecommendedItem>> GetModRecommended()
        {
            const string xpath = "//div[@id='portal_block_722_content']/div/div[@class='portal_li']";
            var node = await GetNode();
            var re = new List<News.RecommendedItem>();

            //找出Mod推荐
            foreach (var selectNode in node.SelectNodes(xpath))
            {
                var modRecommended = new News.RecommendedItem();
                //遍历portal_li元素的所有子元素
                foreach (var htmlNode in selectNode.SelectNodes("child::*"))
                    //遍历该子元素的所有attribute
                foreach (var attribute in htmlNode.Attributes)
                {
                    //如果该子元素有一个href属性，它的值就是要添加的链接
                    if (attribute.Name == "href")
                        modRecommended.Link = $"{Forum}/{attribute.Value.TrimStart('/')}";

                    switch (attribute.Value)
                    {
                        //如果该子元素的attribute的值为portal_li_bottom则可以定位至portal_li_bottom这个div
                        case "portal_li_bottom":
                        {
                            foreach (var selectNode1 in htmlNode.SelectNodes("child::*"))
                                //这个div有两个div作为子元素，以class这个attribute的值作为区分
                                if (selectNode1.GetAttributeValue("class", "") == "portal_li_bl")
                                    modRecommended.Author = selectNode1.InnerText;
                                else
                                    modRecommended.CommentCount = selectNode1.InnerText;
                            break;
                        }
                        case "portal_li_img":
                        {
                            var imgNode = htmlNode.SelectSingleNode("img");
                            modRecommended.Image = imgNode.GetAttributeValue("src", "").Trim();
                            modRecommended.Title = imgNode.GetAttributeValue("alt", "");
                            break;
                        }
                    }
                }

                re.Add(modRecommended);
            }

            return re;
        }
        
        public static async Task<IEnumerable<News.RecommendedItem>> GetPluginRecommended()
        {
            const string xpath = "//div[@id='portal_block_723_content']/div/div[@class='portal_li']";
            var node = await GetNode();
            var re = new List<News.RecommendedItem>();

            //找出Mod推荐
            foreach (var selectNode in node.SelectNodes(xpath))
            {
                var pluginRecommended = new News.RecommendedItem();
                //遍历portal_li元素的所有子元素
                foreach (var htmlNode in selectNode.SelectNodes("child::*"))
                    //遍历该子元素的所有attribute
                foreach (var attribute in htmlNode.Attributes)
                {
                    //如果该子元素有一个href属性，它的值就是要添加的链接
                    if (attribute.Name == "href")
                        pluginRecommended.Link = $"{Forum}/{attribute.Value.TrimStart('/')}";

                    switch (attribute.Value)
                    {
                        //如果该子元素的attribute的值为portal_li_bottom则可以定位至portal_li_bottom这个div
                        case "portal_li_bottom":
                        {
                            foreach (var selectNode1 in htmlNode.SelectNodes("child::*"))
                                //这个div有两个div作为子元素，以class这个attribute的值作为区分
                                if (selectNode1.GetAttributeValue("class", "") == "portal_li_bl")
                                    pluginRecommended.Author = selectNode1.InnerText;
                                else
                                    pluginRecommended.CommentCount = selectNode1.InnerText;
                            break;
                        }
                        case "portal_li_img":
                        {
                            var imgNode = htmlNode.SelectSingleNode("img");
                            pluginRecommended.Image = imgNode.GetAttributeValue("src", "").Trim();
                            pluginRecommended.Title = imgNode.GetAttributeValue("alt", "");
                            break;
                        }
                    }
                }

                re.Add(pluginRecommended);
            }

            return re;
        }
        
        public static async Task<IEnumerable<News.PacksItem>> GetModPackRecommended()
        {
            const string xpath =
                "//div[@id='portal_block_831_content']/div[@class='modpack']/div[@class='portal_txt_li']";
            var re = new List<News.PacksItem>();
            var nodeAsync = await GetNode();

            foreach (var node in nodeAsync.SelectNodes(xpath))
            {
                var modPackItem = new News.PacksItem();
                foreach (var htmlNode in node.SelectNodes("child::*"))
                {
                    //定位至a元素
                    if (htmlNode.Name == "a")
                    {
                        //获取帖子链接
                        modPackItem.Link = $"{Forum}/{htmlNode.GetAttributeValue("href", "").TrimStart('/')}";
                        var imgNode = htmlNode.SelectSingleNode("img");

                        //获取图片链接
                        modPackItem.Image = imgNode.GetAttributeValue("src", "");

                        //获取帖子标题
                        modPackItem.Title = imgNode.GetAttributeValue("title", "");
                    }

                    if (htmlNode.Name != "div") continue;
                    var authorNode =
                        htmlNode.SelectSingleNode("div[@class='portal_txt_name']/div[@class='portal_txt_bl']");

                    //获取帖子作者
                    modPackItem.Author = authorNode.InnerText;
                }

                re.Add(modPackItem);
            }

            return re;
        }
        
        public static async Task<IEnumerable<News.PacksItem>> GetServerPackRecommended()
        {
            const string xpath =
                "//div[@id='portal_block_832_content']/div[@class='modpack']/div[@class='portal_txt_li']";
            var re = new List<News.PacksItem>();
            var nodeAsync = await GetNode();

            foreach (var node in nodeAsync.SelectNodes(xpath))
            {
                var modPackItem = new News.PacksItem();
                foreach (var htmlNode in node.SelectNodes("child::*"))
                {
                    //定位至a元素
                    if (htmlNode.Name == "a")
                    {
                        modPackItem.Link = $"{Forum}/{htmlNode.GetAttributeValue("href", "").TrimStart('/')}";
                        var imgNode = htmlNode.SelectSingleNode("img");

                        //获取图片链接
                        modPackItem.Image = imgNode.GetAttributeValue("src", "");
                    }

                    if (htmlNode.Name != "div") continue;
                    var linkNode = htmlNode.SelectNodes("a/div[@class='portal_txt_title']/p");
                    foreach (var htmlNode1 in linkNode)
                        if (!string.IsNullOrEmpty(htmlNode1.InnerText))
                            //获取帖子标题
                            modPackItem.Title = htmlNode1.InnerText;

                    var authorNode =
                        htmlNode.SelectSingleNode("div[@class='portal_txt_name']/div[@class='portal_txt_bl']");

                    //获取帖子作者
                    modPackItem.Author = authorNode.InnerText;
                }

                re.Add(modPackItem);
            }

            return re;
        }
        
        public static async Task<IEnumerable<News.RecommendedItem>> GetSkinRecommended()
        {
            const string xpath =
                "//div[@id='portal_block_735_content']/div[@class='portal_txt']/div[@class='portal_txt_li']";
            var re = new List<News.RecommendedItem>();
            var nodeAsync = await GetNode();

            foreach (var node in nodeAsync.SelectNodes(xpath))
            {
                var skinsItem = new News.RecommendedItem();
                foreach (var htmlNode in node.SelectNodes("child::*"))
                {
                    //定位至a元素
                    if (htmlNode.Name == "a")
                    {
                        skinsItem.Link = $"{Forum}/{htmlNode.GetAttributeValue("href", "").TrimStart('/')}";
                        var imgNode = htmlNode.SelectSingleNode("img");

                        //获取图片链接
                        skinsItem.Image = imgNode.GetAttributeValue("src", "");
                    }

                    if (htmlNode.Name != "div") continue;
                    var linkNode = htmlNode.SelectNodes("a/div[@class='portal_txt_title']/p");
                    foreach (var htmlNode1 in linkNode)
                        if (!string.IsNullOrEmpty(htmlNode1.InnerText))
                            //获取帖子标题
                            skinsItem.Title = htmlNode1.InnerText;

                    var authorNode =
                        htmlNode.SelectSingleNode("div[@class='portal_txt_name']/div[@class='portal_txt_bl']");

                    //获取帖子作者
                    skinsItem.Author = authorNode.InnerText;

                    var commentsNode =
                        htmlNode.SelectSingleNode("div[@class='portal_txt_name']/div[@class='portal_txt_br']");

                    //获取帖子评论数
                    skinsItem.CommentCount = commentsNode.InnerText;
                }

                re.Add(skinsItem);
            }

            return re;
        }
        
        public static async Task<IEnumerable<News.RecommendedItem>> GetTexturePackRecommended()
        {
            const string xpath =
                "//div[@id='portal_block_833_content']/div[@class='portal_txt']/div[@class='portal_txt_li']";
            var re = new List<News.RecommendedItem>();
            var nodeAsync = await GetNode();

            foreach (var node in nodeAsync.SelectNodes(xpath))
            {
                var texturePackItem = new News.RecommendedItem();
                foreach (var htmlNode in node.SelectNodes("child::*"))
                {
                    //定位至a元素
                    if (htmlNode.Name == "a")
                    {
                        texturePackItem.Link = $"{Forum}/{htmlNode.GetAttributeValue("href", "").TrimStart('/')}";
                        var imgNode = htmlNode.SelectSingleNode("img");

                        //获取图片链接
                        texturePackItem.Image = imgNode.GetAttributeValue("src", "");
                    }

                    if (htmlNode.Name != "div") continue;
                    var linkNode = htmlNode.SelectNodes("a/div[@class='portal_txt_title']/p");
                    foreach (var htmlNode1 in linkNode)
                        if (!string.IsNullOrEmpty(htmlNode1.InnerText))
                            //获取帖子标题
                            texturePackItem.Title = htmlNode1.InnerText;

                    var authorNode =
                        htmlNode.SelectSingleNode("div[@class='portal_txt_name']/div[@class='portal_txt_bl']");

                    //获取帖子作者
                    texturePackItem.Author = authorNode.InnerText;

                    var commentsNode =
                        htmlNode.SelectSingleNode("div[@class='portal_txt_name']/div[@class='portal_txt_br']");

                    //获取帖子评论数
                    texturePackItem.CommentCount = commentsNode.InnerText;
                }

                re.Add(texturePackItem);
            }

            return re;
        }

        public static async Task<IEnumerable<News.RecommendedItem>> GetMapRecommended()
        {
            const string xpath =
                "//div[@id='portal_block_725_content']/div[@class='portal_dev']/div[@class='portal_li']";

            var re = new List<News.RecommendedItem>();
            var nodeAsync = await GetNode();

            foreach (var selectNode in nodeAsync.SelectNodes(xpath))
            {
                var mapRecommendedItem = new News.RecommendedItem();
                foreach (var node in selectNode.SelectNodes("child::*"))
                {
                    if (node.Name == "a")
                    {
                        mapRecommendedItem.Link = $"{Forum}/{node.GetAttributeValue("href", "").TrimStart('/')}";
                        var className = node.GetAttributeValue("class", "");

                        switch (className)
                        {
                            case "portal_li_img":
                            {
                                var imgNode = node.SelectSingleNode("img");
                                mapRecommendedItem.Image = imgNode.GetAttributeValue("src", "");
                                break;
                            }
                            case "portal_li_title":
                                mapRecommendedItem.Title = node.InnerText;
                                break;
                        }
                    }

                    if (node.Name != "div") continue;
                    {
                        foreach (var htmlNode in node.SelectNodes("div"))
                        {
                            var className = htmlNode.GetAttributeValue("class", "");
                            switch (className)
                            {
                                case "portal_li_bl":
                                    mapRecommendedItem.Author = htmlNode.InnerText;
                                    break;
                                case "portal_li_br":
                                    mapRecommendedItem.CommentCount = htmlNode.InnerText;
                                    break;
                            }
                        }
                    }
                }

                re.Add(mapRecommendedItem);
            }


            return re;
        }
        
        public static async Task<IEnumerable<News.CarouselItem>> GetCarouselNews()
        {
            return await GetCarousel("slideshow_3");
        }

        public static async Task<IEnumerable<News.CarouselItem>> GetSkinCarousel()
        {
            return await GetCarousel("portal_wrapper2");
        }

        public static async Task<IEnumerable<News.CarouselItem>> GetTextureCarousel()
        {
            return await GetCarousel("portal_wrapper3");
        }
    }
}