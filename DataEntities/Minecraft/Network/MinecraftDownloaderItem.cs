namespace ModuleLauncher.Re.DataEntities.Minecraft.Network
{
    public class MinecraftDownloaderItem
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public MinecraftDownloadLinkEntity Link { get; set; }
        public string ReleaseTime { get; set; }
    }
}