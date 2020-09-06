using System;
using System.Collections.Generic;
using System.Linq;
using Masuit.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    //head
    public partial class LibrariesLocator
    {
        public MinecraftLocator Locator { get; set; }
        public LibrariesLocator(MinecraftLocator location = null)
        {
            Locator = location;
        }
    }
    
    //exposed
    public partial class LibrariesLocator
    {
        
    }
    
    //inside
    public partial class LibrariesLocator
    {
        private IEnumerable<string> GetLibraryNames(string name)
        {
            try
            {
                var entity = Locator.GetMinecraftEntity(name);
                return entity.libraries.Select(x => x["name"]?.ToString());
            }
            catch (Exception e)
            {
                throw new Exception($"获取minecraft实体失败\n{e.Message}");
            }
        }
    }
}