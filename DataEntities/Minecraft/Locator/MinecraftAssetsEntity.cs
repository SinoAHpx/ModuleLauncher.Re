namespace ModuleLauncher.Re.DataEntities.Minecraft.Locator
{
    public class MinecraftAssetsEntity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Link { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}