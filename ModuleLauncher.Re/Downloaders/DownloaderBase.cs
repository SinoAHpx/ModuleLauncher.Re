using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Downloader;
using MoreLinq.Extensions;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

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

        public Action<DownloadStartedEventArgs> DownloadStarted { get; set; }

        public Action<AsyncCompletedEventArgs> DownloadCompleted { get; set; }

        public Action<DownloadProgressChangedEventArgs> DownloadProgressChanged { get; set; }
        
        /// <summary>
        /// Download files in collection one-by-one
        /// </summary>
        protected virtual async Task Download()
        {
            var configuration = new DownloadConfiguration();
            
            foreach (var (url, file) in Files)
            {
                var downloader = new DownloadService(configuration);

                #region Invoking event handlers

                downloader.DownloadStarted += (sender, args) =>
                {
                    DownloadStarted?.Invoke(args);
                };
                downloader.DownloadFileCompleted += (sender, args) =>
                {
                    DownloadCompleted?.Invoke(args);
                };
                downloader.DownloadProgressChanged += (sender, args) =>
                {
                    DownloadProgressChanged?.Invoke(args);
                };

                #endregion

                await downloader.DownloadFileTaskAsync(url, file.FullName);
            }
        }

        /// <summary>
        /// Download single file
        /// </summary>
        /// <param name="file"></param>
        private async Task Download((string, FileInfo) file)
        {
            var configuration = new DownloadConfiguration();
            
            var downloader = new DownloadService(configuration);

            #region Invoking event handlers

            downloader.DownloadStarted += (sender, args) =>
            {
                DownloadStarted?.Invoke(args);
            };
            downloader.DownloadFileCompleted += (sender, args) =>
            {
                DownloadCompleted?.Invoke(args);
            };
            downloader.DownloadProgressChanged += (sender, args) =>
            {
                DownloadProgressChanged?.Invoke(args);
            };

            #endregion

            var (url, fileInfo) = file;
            
            await downloader.DownloadFileTaskAsync(url, fileInfo.FullName);
        }

        /// <summary>
        /// Parallel download files in colletion
        /// </summary>
        /// <param name="count"></param>
        protected virtual async Task DownloadParallel(int count = 5)
        {
            var subFiles = Files.Batch(count);

            foreach (var tuples in subFiles)
            {
                var tasks = tuples.Select(Download).ToList();

                await Task.WhenAll(tasks);
            }
        }
    }
}