using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Re.Downloaders;
using ModuleLauncher.Re.Models.Downloaders;
using ModuleLauncher.Re.Models.Downloaders.Minecraft;
using ModuleLauncher.Re.Utils.Extensions;
using MoreLinq;
using ReactiveUI;

namespace ModuleLauncher.Example.ViewModels.Downloaders
{
    public class DownloadersViewModel : ViewModelBase
    {
        #region Minecraft root

        private string _root;

        public string Root
        {
            get => _root;
            set => this.RaiseAndSetIfChanged(ref _root, value);
        }
        public async void Browser()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Browser .minecraft directory",
                Directory = @"C:\Users\ahpx\AppData\Roaming"
            };

            Root = await dialog.ShowAsync(GlobalUtility.GetMainWindow());
        }

        #endregion

        #region Minecrafts

        public ObservableCollection<MinecraftDownloadItem> Minecrafts { get; set; } = new ();

        private MinecraftDownloadItem _selectMc;

        public MinecraftDownloadItem SelectMc
        {
            get => _selectMc;
            set => this.RaiseAndSetIfChanged(ref _selectMc, value);
        }

        public async void GetMinecrafts()
        {
            try
            {
                var downloader = new MinecraftDownloader(Root);

                var items = await downloader.GetRemoteMinecrafts();

                items.ForEach(x => Minecrafts.Add(x));
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }
        }

        public async void GetLatest()
        {
            try
            {
                var downloader = new MinecraftDownloader(Root);

                var items = await downloader.GetLatestVersions();

                await MessageBoxEx.Show(items.ToJsonString());
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }
        }

        #endregion

        #region Downloads

        private double _mcDownloadProgress;

        public double McDownloadProgress
        {
            get => _mcDownloadProgress;
            set => this.RaiseAndSetIfChanged(ref _mcDownloadProgress, value);
        }

        public async void DownloadMc(string downloadSource)
        {
            try
            {
                var downloader = new MinecraftDownloader(Root)
                {
                    Source = downloadSource switch
                    {
                        "Official" => DownloaderSource.Mojang,
                        "Bmclapi" => DownloaderSource.Bmclapi,
                        "Mcbbs" => DownloaderSource.Mcbbs,
                        _ => DownloaderSource.Mojang
                    }
                };

                downloader.DownloadProgressChanged += args =>
                {
                    McDownloadProgress = args.ProgressPercentage;
                };

                downloader.DownloadCompleted += async args =>
                {
                    await Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        await MessageBoxEx.Show($"{SelectMc.Id} download complete!");

                        McDownloadProgress = 0;
                    });
                };

                await downloader.Download(SelectMc.Id);
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }
        }

        #endregion
    }
}