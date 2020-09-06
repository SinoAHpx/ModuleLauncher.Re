namespace ModuleLauncher.Re.DataEntities.Minecraft.Locator
{
    /// <summary>
    /// 用于解析Minecraft json文件的实体类
    /// </summary>
    public class MinecraftFileEntity
    {
        public string Jar { get; set; }
        public string Json { get; set; }
        public string Native { get; set; }
        public string Root { get; set; }
        public string Name { get; set; }
    }
}