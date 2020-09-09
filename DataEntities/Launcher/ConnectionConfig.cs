namespace ModuleLauncher.Re.DataEntities.Launcher
{
    public class ConnectionConfig
    {
        private string _port;
        public string IpAddress { get; set; }

        public string Port
        {
            get => _port;
            set => _port = value ?? "25565";
        }

        public static implicit operator ConnectionConfig(string ip)
        {
            return new ConnectionConfig
            {
                IpAddress = ip,
                Port  = "25565"
            };
        }
    }
}