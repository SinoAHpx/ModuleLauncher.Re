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
    public class DownloadUtility
    {
        private readonly string UserAgent = $"ModuleLauncher.Re/{SystemUtility.GetAssemblyVersion()}";
        
        public Action<DownloadStartedEventArgs> DownloadStarted { get; set; }

        public Action<AsyncCompletedEventArgs> DownloadCompleted { get; set; }

        public Action<DownloadProgressChangedEventArgs> DownloadProgressChanged { get; set; }

        public Action<Exception, int> OnRetry { get; set; }

        public (string url, FileInfo file) DownloadInfo { get; set; }

        private DownloadService _service;

        public async Task Download()
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
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5),
                }, (exception, time, count, _) =>
                {
                    OnRetry?.Invoke(exception, count);
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