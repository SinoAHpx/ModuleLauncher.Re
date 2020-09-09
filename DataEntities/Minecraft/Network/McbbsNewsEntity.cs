namespace ModuleLauncher.Re.DataEntities.Minecraft.Network
{
    public class RecommendedItem
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string DisplayTitle { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }
        public string CommentCount { get; set; }
    }

    public class CarouselItem
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }

    public class PacksItem
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }
    }
}