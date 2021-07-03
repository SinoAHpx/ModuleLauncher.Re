using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Re.Downloaders;
using ModuleLauncher.Re.Downloaders.Concrete;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Downloaders;
using ModuleLauncher.Re.Models.Downloaders.Minecraft;
using ModuleLauncher.Re.Models.Locators.Dependencies;
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
                        "Official" => DownloaderSource.Official,
                        "Bmclapi" => DownloaderSource.Bmclapi,
                        "Mcbbs" => DownloaderSource.Mcbbs,
                        _ => DownloaderSource.Official
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
                
                await MessageBoxEx.Show($"{SelectMc.Id} download complete!");
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }
        }

        #endregion
        
        #region Download libraries

        public ObservableCollection<DependencyDownloaderItemViewModel> Libraries { get; set; } = new();

        public async void GetLibraries()
        {
            try
            {
                Libraries.Clear();

                var minecraftLocator = new MinecraftLocator(Root);
                var librariesLocator = new LibrariesLocator(minecraftLocator);

                if (!(await minecraftLocator.GetLocalMinecraft(SelectMc.Id)).Locality.Jar.Exists)
                {
                    await MessageBoxEx.Show($"Please download {SelectMc.Id} first!");
                }

                var dependencies = await librariesLocator.GetDependencies(SelectMc.Id);

                foreach (var dependency in dependencies)
                {
                    Libraries.Add(new()
                    {
                        Dependency = dependency
                    });
                }
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }

        }

        public async void DownloadLibraries(string downloadSource)
        {
            try
            {
                var libraries = new List<DependencyDownloaderItemViewModel>(Libraries);
                
                foreach (var viewModels in libraries.Batch(5))
                {
                    var tasks = new List<Task>();
                    foreach (var viewModel in viewModels)
                    {
                        if (viewModel.Dependency.File.Exists)
                        {
                            lock (Libraries)
                            {
                                Libraries.Remove(viewModel);
                            }
                            
                            continue;
                        }
                        
                        var downloader = new LibrariesDownloader(Root);

                        downloader.DownloadProgressChanged += args =>
                        {
                            lock (Libraries)
                            {
                                Libraries.Single(x => x.Dependency.Name == viewModel.Dependency.Name).Progress =
                                    args.ProgressPercentage;
                            }
                        };
                        downloader.DownloadCompleted += args =>
                        {
                            lock (Libraries)
                            {
                                Libraries.Remove(viewModel);
                            }
                        };

                        tasks.Add(downloader.Download(viewModel.Dependency));
                    }

                    await Task.WhenAll(tasks);
                }

                await MessageBoxEx.Show("Download complete!");
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }
        }

        #endregion

        #region Download assets

        public ObservableCollection<DependencyDownloaderItemViewModel> Assets { get; set; } = new();

        public async void GetAssets()
        {
            try
            {
                Assets.Clear();

                var minecraftLocator = new MinecraftLocator(Root);
                var assetsLocator = new AssetsLocator(minecraftLocator);

                if (!(await minecraftLocator.GetLocalMinecraft(SelectMc.Id)).Locality.Jar.Exists)
                {
                    await MessageBoxEx.Show($"Please download {SelectMc.Id} first!");
                }

                var dependencies = await assetsLocator.GetDependencies(SelectMc.Id);

                foreach (var dependency in dependencies)
                {
                    Assets.Add(new()
                    {
                        Dependency = dependency
                    });
                }
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }

        }

        public async void DownloadAssets(string downloadSource)
        {
            try
            {
                var assets = new List<DependencyDownloaderItemViewModel>(Assets);
                
                foreach (var viewModels in assets.Batch(5))
                {
                    var tasks = new List<Task>();
                    foreach (var viewModel in viewModels)
                    {
                        if (viewModel.Dependency.File.Exists)
                        {
                            lock (Assets)
                            {
                                Assets.Remove(viewModel);
                            }
                            
                            continue;
                        }
                        
                        var downloader = new AssetsDownloader(Root);

                        downloader.DownloadProgressChanged += args =>
                        {
                            lock (Assets)
                            {
                                Assets.Single(x => x.Dependency.Name == viewModel.Dependency.Name).Progress =
                                    args.ProgressPercentage;
                            }
                        };
                        downloader.DownloadCompleted += args =>
                        {
                            lock (Assets)
                            {
                                Assets.Remove(viewModel);
                            }
                        };

                        tasks.Add(downloader.Download(viewModel.Dependency));
                    }

                    await Task.WhenAll(tasks);
                }

                await MessageBoxEx.Show("Download complete!");
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }
        }

        #endregion
    }
}