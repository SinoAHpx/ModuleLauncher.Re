using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Downloader;

namespace ModuleLauncher.Re.Downloaders
{
    /// <summary>
    /// This is not a static class，
    /// Each download request should be a new object of this
    /// </summary>
    public abstract class DownloaderBase
    {
        /// <summary>
        /// item1 is url, item2 is local file info
        /// </summary>
        protected abstract List<(string, FileInfo)> Files { get; set; }

        /// <summary>
        /// Download files in collection one-by-one
        /// </summary>
        public virtual async Task Download()
        {
            var configuration = new DownloadConfiguration();
            
            foreach (var (url, file) in Files)
            {
                var downloader = new DownloadService(configuration);

                downloader.DownloadStarted += DownloaderOnDownloadStarted;
                downloader.DownloadFileCompleted += DownloaderOnDownloadFileCompleted;
                downloader.DownloadProgressChanged += DownloaderOnDownloadProgressChanged;
                
                await downloader.DownloadFileTaskAsync(url, file.FullName);
            }
        }

        protected abstract void DownloaderOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e);

        protected abstract void DownloaderOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e);

        protected abstract void DownloaderOnDownloadStarted(object sender, DownloadStartedEventArgs e);
    }
}