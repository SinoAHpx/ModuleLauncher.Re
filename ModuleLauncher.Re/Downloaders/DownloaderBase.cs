using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Downloader;
using ModuleLauncher.Re.Utils;
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
        public Action<DownloadStartedEventArgs> DownloadStarted { get; set; }

        public Action<AsyncCompletedEventArgs> DownloadCompleted { get; set; }

        public Action<DownloadProgressChangedEventArgs> DownloadProgressChanged { get; set; }

        public Action<Exception, int> OnRetry { get; set; }

        /// <summary>
        /// Download single file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ignoreExist"></param>
        public async Task Download((string, FileInfo) file, bool ignoreExist = false)
        {
            if (file.Item2.Exists && !ignoreExist)
                return;
            
            var downloader = new DownloadUtility
            {
                DownloadInfo = file
            };

            #region Invoking event handlers

            downloader.DownloadStarted += args =>
            {
                DownloadStarted?.Invoke(args);
            };
            downloader.DownloadCompleted += args =>
            {
                DownloadCompleted?.Invoke(args);
            };
            downloader.DownloadProgressChanged += args =>
            {
                DownloadProgressChanged?.Invoke(args);
            };
            downloader.OnRetry += (exception, i) =>
            {
                OnRetry?.Invoke(exception, i);
            };

            #endregion

            await downloader.Download();
        }

        /// <summary>
        /// Parallel download files in colletion
        /// </summary>
        /// <param name="files"></param>
        /// <param name="ignoreExist"></param>
        /// <param name="count"></param>
        protected virtual async Task DownloadParallel(IEnumerable<(string, FileInfo)> files, bool ignoreExist = false, int count = 5)
        {
            var subFiles = files.Batch(count);

            foreach (var tuples in subFiles)
            {
                var tasks = tuples.Select(x => Download(x, ignoreExist)).ToList();

                await Task.WhenAll(tasks);
            }
        }
    }
}