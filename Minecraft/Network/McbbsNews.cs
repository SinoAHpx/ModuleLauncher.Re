using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.DataEntities.Minecraft.Network;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;
using ModuleLauncher.Re.Utils.Inside;

namespace ModuleLauncher.Re.Minecraft.Network
{
    public partial class McbbsNews
    {
        private const string Forum = "https://www.mcbbs.net";
    }
    
    //async
    public partial class McbbsNews
    {
        public static async Task<IEnumerable<RecommendedItem>> GetModeratorRecommendedAsync()
        {
            const string xpath = "//div[@id='portal_block_729_content']/div/div[@class='portal_li']";
            var node = await McbbsHelper.GetNodeAsync();
            var re = new List<RecommendedItem>();

            //找出版主推荐
            foreach (var selectNode in node.SelectNodes(xpath))
            {
                var moderatorRecommended = new RecommendedItem();
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
        
        public static async Task<IEnumerable<RecommendedItem>> GetModRecommendedAsync()
        {
            const string xpath = "//div[@id='portal_block_722_content']/div/div[@class='portal_li']";
            var node = await McbbsHelper.GetNodeAsync();
            var re = new List<RecommendedItem>();

            //找出Mod推荐
            foreach (var selectNode in node.SelectNodes(xpath))
            {
                var modRecommended = new RecommendedItem();
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

        public static async Task<IEnumerable<RecommendedItem>> GetPluginRecommendedAsync()
        {
            const string xpath = "//div[@id='portal_block_723_content']/div/div[@class='portal_li']";
            var node = await McbbsHelper.GetNodeAsync();
            var re = new List<RecommendedItem>();

            //找出Mod推荐
            foreach (var selectNode in node.SelectNodes(xpath))
            {
                var pluginRecommended = new RecommendedItem();
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

        public static async Task<IEnumerable<PacksItem>> GetModPackRecommendedAsync()
        {
            const string xpath =
                "//div[@id='portal_block_831_content']/div[@class='modpack']/div[@class='portal_txt_li']";
            var re = new List<PacksItem>();
            var nodeAsync = await McbbsHelper.GetNodeAsync();
            
            foreach (var node in nodeAsync.SelectNodes(xpath))
            {
                var modPackItem = new PacksItem();
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

        public static async Task<IEnumerable<PacksItem>> GetServerPackRecommendedAsync()
        {
            const string xpath =
                "//div[@id='portal_block_832_content']/div[@class='modpack']/div[@class='portal_txt_li']";
            var re = new List<PacksItem>();
            var nodeAsync = await McbbsHelper.GetNodeAsync();
            
            foreach (var node in nodeAsync.SelectNodes(xpath))
            {
                var modPackItem = new PacksItem();
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

        public static async Task<IEnumerable<RecommendedItem>> GetSkinRecommendedAsync()
        {
            const string xpath =
                "//div[@id='portal_block_735_content']/div[@class='portal_txt']/div[@class='portal_txt_li']";
            var re = new List<RecommendedItem>();
            var nodeAsync = await McbbsHelper.GetNodeAsync();
            
            foreach (var node in nodeAsync.SelectNodes(xpath))
            {
                var skinsItem = new RecommendedItem();
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

        public static async Task<IEnumerable<RecommendedItem>> GetTexturePackRecommendedAsync()
        {
            const string xpath =
                "//div[@id='portal_block_833_content']/div[@class='portal_txt']/div[@class='portal_txt_li']";
            var re = new List<RecommendedItem>();
            var nodeAsync = await McbbsHelper.GetNodeAsync();
            
            foreach (var node in nodeAsync.SelectNodes(xpath))
            {
                var texturePackItem = new RecommendedItem();
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

        public static async Task<IEnumerable<RecommendedItem>> GetMapRecommendedAsync()
        {
            const string xpath =
                "//div[@id='portal_block_725_content']/div[@class='portal_dev']/div[@class='portal_li']";

            var re = new List<RecommendedItem>();
            var nodeAsync = await McbbsHelper.GetNodeAsync();
            
            foreach (var selectNode in nodeAsync.SelectNodes(xpath))
            {
                var mapRecommendedItem = new RecommendedItem();
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

        public static async Task<IEnumerable<CarouselItem>> GetCarouselNewsAsync() => await GetCarouselAsync("slideshow_3");

        public static async Task<IEnumerable<CarouselItem>> GetSkinCarouselAsync() => await GetCarouselAsync("portal_wrapper2");

        public static async Task<IEnumerable<CarouselItem>> GetTextureCarouselAsync() => await GetCarouselAsync("portal_wrapper3");
    }
    
    //private
    public partial class McbbsNews
    {
        private static async Task<IEnumerable<CarouselItem>> GetCarouselAsync(string id)
        {
            var xpath = $"//div[@id='{id}']/div[@class='slideshow_item']/div[@class='image']/a";
            var nodeAsync = await McbbsHelper.GetNodeAsync();

            return (from node in nodeAsync.SelectNodes(xpath)
                let imgNode = node.SelectSingleNode("img")
                select new CarouselItem
                {
                    Link = $"{Forum}/{node.GetAttributeValue("href", "").TrimStart('/')}",
                    Title = node.GetAttributeValue("title", ""), Image = imgNode.GetAttributeValue("src", "")
                });
        }
    }
    
    //sync
    public partial class McbbsNews
    {
        public static IEnumerable<RecommendedItem> GetModeratorRecommended() => GetModeratorRecommendedAsync().GetResult();
        public static IEnumerable<RecommendedItem> GetModRecommended() => GetModRecommendedAsync().GetResult();
        public static IEnumerable<RecommendedItem> GetPluginRecommended() => GetModRecommendedAsync().GetResult();
        public static IEnumerable<RecommendedItem> GetSkinRecommended()=> GetSkinRecommendedAsync().GetResult();
        public static IEnumerable<RecommendedItem> GetTexturePackRecommended()=> GetTexturePackRecommendedAsync().GetResult();
        public static IEnumerable<RecommendedItem> GetMapRecommended()=> GetMapRecommendedAsync().GetResult();
        public static IEnumerable<CarouselItem> GetCarouselNews()=> GetCarouselNewsAsync().GetResult();
        public static IEnumerable<CarouselItem> GetSkinCarousel()=> GetSkinCarouselAsync().GetResult();
        public static IEnumerable<CarouselItem> GetTextureCarousel()=> GetTextureCarouselAsync().GetResult();
        public static IEnumerable<PacksItem> GetModPackRecommended()=> GetModPackRecommendedAsync().GetResult();
        public static IEnumerable<PacksItem> GetServerPackRecommended()=> GetServerPackRecommendedAsync().GetResult();
    }
}