namespace ModuleLauncher.Re.DataEntities.Launcher
{
    public class ResolutionConfig
    {
        public string WindowWidth { get; set; }
        public string WindowHeight { get; set; }
        public bool? FullScreen { get; set; }

        public static implicit operator ResolutionConfig(string size)
        {
            return new ResolutionConfig
            {
                WindowHeight = size,
                WindowWidth = size,
                FullScreen = false
            };
        }
    }
}