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
            var props = this.GetType().GetProperties();

            return props.Where(info => info.GetAccessors(false)[0].IsPublic)
                .Aggregate(string.Empty,
                (current, info) => current + $"PROP_NAME:{info.Name}\nPROP_VALUE:\n{info.GetValue(this)}\n");
        }
    }
}