using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.DataEntities.Minecraft.Locator
{
    //universal
    internal partial class MinecraftJsonEntity
    {
        internal JObject assetIndex { get; set; }
        internal string assets { get; set; }
        internal string id { get; set; }
        internal JArray libraries { get; set; }
        internal string mainClass { get; set; }
        internal string type { get; set; }
    }
    
    //modify
    internal partial class MinecraftJsonEntity
    {
        internal string inheritsFrom { get; set; }
    }
    
    //new
    internal partial class MinecraftJsonEntity
    {
        internal JObject arguments { get; set; }
    }
    
    //old
    internal partial class MinecraftJsonEntity
    {
        internal string minecraftArguments { get; set; }
    }
}