using System;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Downloader;
using Polly;
using DownloadProgressChangedEventArgs = Downloader.DownloadProgressChangedEventArgs;

namespace ModuleLauncher.Re.Utils
{
    /// <summary>
    /// Download utilities
    /// </summary>
    internal class DownloadUtility
    {
        private readonly string UserAgent = $"ModuleLauncher.Re/{SystemUtility.GetAssemblyVersion()}";
        
        internal Action<DownloadStartedEventArgs> DownloadStarted { get; set; }

        internal Action<AsyncCompletedEventArgs> DownloadCompleted { get; set; }

        internal Action<DownloadProgressChangedEventArgs> DownloadProgressChanged { get; set; }

        internal Action<Exception, int> OnRetry { get; set; }

        internal (string url, FileInfo file) DownloadInfo { get; set; }

        private DownloadService _service;

        internal async Task Download()
        {
            var configuration = new DownloadConfiguration
            {
                ParallelDownload = true,
                MaxTryAgainOnFailover = 114514,
                OnTheFlyDownload = true,
                RequestConfiguration = new RequestConfiguration
                {
                    UserAgent = UserAgent
                }
            };

            var policy = Policy
                .Handle<Exception>()
                .RetryAsync(3, (exception, i) =>
                {
                    OnRetry?.Invoke(exception, i);
                });

            try
            {
                await policy.ExecuteAsync(async () =>
                {
                    using (_service = new DownloadService(configuration))
                    {
                        _service.DownloadStarted += (sender, args) => { DownloadStarted?.Invoke(args); };
                        _service.DownloadProgressChanged += (sender, args) =>
                        {
                            DownloadProgressChanged?.Invoke(args);
                        };
                        _service.DownloadFileCompleted += (sender, args) => { DownloadCompleted?.Invoke(args); };

                        await _service.DownloadFileTaskAsync(DownloadInfo.url, DownloadInfo.file.FullName).ConfigureAwait(false);
                    }
                });
            }
            catch (Exception e)
            {
                throw new WebException("Download failed!", e);
            }
        }

        public void Cancel()
        {
            _service.CancelAsync();
        }
    }
}