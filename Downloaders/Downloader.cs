using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Downloaders;
using Downloader;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

namespace AHpx.ModuleLauncher.Downloaders
{
    public class Downloader
    {
        private DownloadService _downloadService;

        public Downloader()
        {
            _downloadService = new DownloadService(new DownloadConfiguration
            {
                ParallelDownload = true,
                RequestConfiguration = new RequestConfiguration
                {
                    AllowAutoRedirect = true,
                    UserAgent = "ModuleLauncher/2.8"
                }
            });

            _downloadService.DownloadStarted += new EventHandler<DownloadStartedEventArgs>(DownloadStared);
            _downloadService.DownloadFileCompleted += new EventHandler<AsyncCompletedEventArgs>(DownloadCompleted);
            _downloadService.DownloadProgressChanged += new EventHandler<DownloadProgressChangedEventArgs>(DownloadProgressChanged);
        }

        public Action<DownloadArgs.StartedArgs> StartedAction { get; set; } = args => { };
        public Action<DownloadArgs.CompletedArgs> CompletedAction { get; set; } = args => { };
        public Action<DownloadArgs.ProgressArgs> ProgressAction { get; set; } = args => { };
        
        public async Task Download(string address, string fileName)
        {
            await _downloadService.DownloadFileTaskAsync(address, fileName);
        }

        private void DownloadStared(object sender, DownloadStartedEventArgs e)
        {
            StartedAction?.Invoke(new DownloadArgs.StartedArgs
            {
                FileName = e.FileName,
                FileSize = e.TotalBytesToReceive
            });
        }
        
        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            CompletedAction?.Invoke(new DownloadArgs.CompletedArgs
            {
                Cancelled = e.Cancelled,
                Exception = e.Error
            });
        }
        
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressAction?.Invoke(new DownloadArgs.ProgressArgs
            {
                AverageBytesPerSecondSpeed = e.AverageBytesPerSecondSpeed,
                BytesPerSecondSpeed = e.BytesPerSecondSpeed,
                Percentage = e.ProgressPercentage,
                ProgressedBytesSize = e.ProgressedByteSize,
                ProgressID = e.ProgressId,
                ReceivedBytes = e.ReceivedBytes,
                ReceivedBytesSize = e.ReceivedBytesSize,
                TotalBytesSize = e.TotalBytesToReceive
            });
        }
    }
}