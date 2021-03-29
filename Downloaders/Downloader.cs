using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AHpx.ModuleLauncher.Data.Downloaders;
using Downloader;
using MoreLinq;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

namespace AHpx.ModuleLauncher.Downloaders
{
    public class Downloader
    {
        public Action<DownloadArgs.StartedArgs> StartedAction { get; set; } = args => { };
        public Action<DownloadArgs.CompletedArgs> CompletedAction { get; set; } = args => { };
        public Action<DownloadArgs.ProgressArgs> ProgressAction { get; set; } = args => { };
        
        public async Task Download(DownloadItem item)
        {
            var downloadService = new DownloadService(new DownloadConfiguration
            {
                ParallelDownload = true,
                RequestConfiguration = new RequestConfiguration
                {
                    AllowAutoRedirect = true,
                    UserAgent = "ModuleLauncher/2.8"
                }
            });

            downloadService.DownloadStarted += DownloadStared;
            downloadService.DownloadFileCompleted += DownloadCompleted;
            downloadService.DownloadProgressChanged += DownloadProgressChanged;
            
            await downloadService.DownloadFileTaskAsync(item.Address, item.FileName);
        }

        public async Task Download(IEnumerable<DownloadItem> items, int maxParallelCount = 3)
        {
            var parallelArr = items.Batch(maxParallelCount);
            var tasks = new List<Task>();
            
            foreach (var item in parallelArr)
            {
                foreach (var downloadItem in item)
                {
                    tasks.Add(Download(downloadItem));
                }

                await Task.WhenAll(tasks);
                tasks.Clear();
            }
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