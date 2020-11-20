using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Downloader;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

namespace AHpx.ModuleLauncher.Downloaders
{
    public class DownloaderCore
    {
        public Action<object, AsyncCompletedEventArgs> OnCompleted { get; set; }
        public Action<object, DownloadProgressChangedEventArgs> OnProgressChanged { get; set; }
        public Action<object, DownloadProgressChangedEventArgs> OnChunkProgressChanged { get; set; }

        public DownloaderCore()
        {
            this.OnCompleted = (o, args) => { };
            this.OnProgressChanged = (o, args) => { };
            this.OnChunkProgressChanged = (o, args) => { };
        }

        public virtual async Task Download(string url, FileInfo file)
        {
            var service = new DownloadService(new DownloadConfiguration
            {
                AllowedHeadRequest = false,
                ParallelDownload = true,
                ChunkCount = 8,
                MaxTryAgainOnFailover = 50,
                OnTheFlyDownload = true,
                RequestConfiguration =
                {
                    UserAgent = $"ModuleLauncher/2.7",
                    KeepAlive = true,
                }
            });
            
            service.DownloadFileCompleted += new EventHandler<AsyncCompletedEventArgs>(OnCompleted);
            service.DownloadProgressChanged += new EventHandler<DownloadProgressChangedEventArgs>(OnProgressChanged);
            service.ChunkDownloadProgressChanged += new EventHandler<DownloadProgressChangedEventArgs>(OnChunkProgressChanged);
            
            await service.DownloadFileAsync(url, file.FullName);
        }
    }
}