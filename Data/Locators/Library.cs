using System.IO;
using System.Linq;

namespace AHpx.ModuleLauncher.Data.Locators
{
    public class Library
    {
        public string Name { get; set; }
        public FileInfo File { get; set; }
        //public string Url { get; set; } TODO: IT SHOULD BE A DownloadInfo CLASS TO INCLUDE URL & SAVE PATH

        public override string ToString()
        {
            return File.FullName;
        }
    }
}