namespace ModuleLauncher.Re.DataEntities.Minecraft.Network
{
    public class MojangServiceStatus
    {
        public string Server { get; set; }

        /// <summary>
        ///     可能的值为green（无问题）、yellow（有些许问题）、red（服务不可用）。
        /// </summary>
        public string Status { get; set; }
    }
}