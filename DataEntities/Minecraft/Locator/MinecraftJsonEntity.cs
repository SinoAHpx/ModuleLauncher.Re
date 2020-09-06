using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.DataEntities.Minecraft.Locator
{
    //universal
    public partial class MinecraftJsonEntity
    {
        public JObject assetIndex { get; set; }
        public string assets { get; set; }
        public string id { get; set; }
        public JArray libraries { get; set; }
        public string mainClass { get; set; }
        public string type { get; set; }
    }
    
    //modify
    public partial class MinecraftJsonEntity
    {
        public string inheritsFrom { get; set; }
    }
    
    //new
    public partial class MinecraftJsonEntity
    {
        public JObject arguments { get; set; }
    }
    
    //old
    public partial class MinecraftJsonEntity
    {
        public string minecraftArguments { get; set; }
    }
}