using System.IO;
using System.Linq;

namespace AHpx.ModuleLauncher.Data.Locators
{
    public class Library
    {
        public string Name { get; set; }
        public FileInfo File { get; set; }
        public string RelativeUrl { get; set; }

        public override string ToString()
        {
            return File.FullName;
        }
    }
}