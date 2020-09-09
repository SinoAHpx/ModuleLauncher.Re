namespace ModuleLauncher.Re.DataEntities.Minecraft.Locator
{
    public class MinecraftLibrariesEntity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Link { get; set; }
        public string[] UnformattedName { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}