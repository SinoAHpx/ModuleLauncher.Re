namespace AHpx.ModuleLauncher.Data.Downloaders
{
    public class DownloadItem
    {
        public string Address { get; set; }
        public string FileName { get; set; }

        public override string ToString()
        {
            return FileName;
        }
    }
}