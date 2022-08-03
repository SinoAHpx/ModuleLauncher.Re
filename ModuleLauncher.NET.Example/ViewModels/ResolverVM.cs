using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaEdit.Utils;
using DynamicData;
using DynamicData.Binding;
using Flurl.Http;
using Manganese.Text;
using ModuleLauncher.NET.Example.Utils;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using MoreLinq;
using Polly;

namespace ModuleLauncher.NET.Example.ViewModels;

public class ResolverVM : ViewModelBase
{
    #region MinecraftResolver

    private ObservableCollection<MinecraftTreeNode> _minecraftTreeItems;

    public ObservableCollection<MinecraftTreeNode> MinecraftTreeItems
    {
        get => _minecraftTreeItems;
        set => this.RaiseAndSetIfChanged(ref _minecraftTreeItems, value);
    }

    public ReactiveCommand<Unit, Unit> UpdateMinecraftTreeCommand { get; set; }

    private async void UpdateMinecraftTree()
    {
        await Task.Run(() =>
        {
            if (SelectedMinecraft == null)
            {
                return;
            }

            MinecraftTreeItems = new();

            var tree = SelectedMinecraft.Tree;
            MinecraftTreeItems.Add(new MinecraftTreeNode(tree.Root)
                .WithSubNodes(new MinecraftTreeNode(tree.Assets).WithSubNodes(tree.AssetsIndexes))
                .WithSubNodes(tree.Libraries, tree.Mods, tree.Saves, tree.ResourcesPacks, tree.TexturePacks)
                .WithSubNodes(new MinecraftTreeNode(tree.VersionRoot).WithSubNodes(tree.Jar, tree.Json, tree.Natives)));
            MinecraftTreeItems.Add(new MinecraftTreeNode(tree.WorkingDirectory));
        });
    }

    public class MinecraftTreeNode
    {
        public string Fullname { get; set; }

        public ObservableCollection<MinecraftTreeNode> SubNodes { get; set; } = new();

        public MinecraftTreeNode(FileSystemInfo info)
        {
            Fullname = info.FullName;
        }

        public MinecraftTreeNode WithSubNodes(params FileSystemInfo[] nodes)
        {
            SubNodes.AddRange(nodes.Select(x => new MinecraftTreeNode(x)));

            return this;
        }
        
        public MinecraftTreeNode WithSubNodes(params MinecraftTreeNode[] nodes)
        {
            SubNodes.AddRange(nodes);

            return this;
        }
    }
    
    private ObservableCollection<MinecraftEntry> _minecraftVersions;

    public ObservableCollection<MinecraftEntry> MinecraftVersions
    {
        get => _minecraftVersions;
        set => this.RaiseAndSetIfChanged(ref _minecraftVersions, value);
    }

    private MinecraftEntry? _selectedMinecraft;

    public MinecraftEntry? SelectedMinecraft
    {
        get => _selectedMinecraft;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMinecraft, value);
            UpdateMinecraftTree();
            RefreshLibraries();
            RefreshAssets();
        }
    }

    public ReactiveCommand<Unit, Unit> RefreshMinecraftVersionsCommand { get; set; }

    private async void RefreshMinecraftVersions()
    {
        if (DataBus.MinecraftResolver != null)
        {
            try
            {
                await Task.Run(() =>
                {
                    var minecrafts = DataBus.MinecraftResolver.GetMinecrafts();

                    //minecraftEntry.ValidateChecksum means check if a minecraft entry is valid
                    MinecraftVersions = new ObservableCollection<MinecraftEntry>(minecrafts);
                });

            }
            catch (Exception e)
            {
                await GeneralUtils.PromptExceptionDialogAsync(e);
            }
        }
        else
        {
            await GeneralUtils.PromptDialogAsync("No versions could be found, have you set your .minecraft path?");
        }
    }

    private void InitializeMinecraftResolverCommands()
    {
        RefreshMinecraftVersionsCommand = ReactiveCommand.Create(RefreshMinecraftVersions);
        UpdateMinecraftTreeCommand = ReactiveCommand.Create(UpdateMinecraftTree);
    }
    
    #endregion

    #region Libraries resolver

    private ObservableCollection<LibraryEntry> _minecraftLibraries;

    public ObservableCollection<LibraryEntry> MinecraftLibraries
    {
        get => _minecraftLibraries;
        set => this.RaiseAndSetIfChanged(ref _minecraftLibraries, value);
    }

    public ReactiveCommand<Unit, Unit> RefreshLibrariesCommand { get; set; }

    private async void RefreshLibraries()
    {
        await Task.Run(() =>
        {
            if (SelectedMinecraft == null)
            {
                return;
            }

            var libraries = SelectedMinecraft.GetLibraries();

            MinecraftLibraries = new(libraries);
        });
    }

    private void InitializeLibrariesResolverCommands()
    {
        RefreshLibrariesCommand = ReactiveCommand.Create(RefreshLibraries);
    }

    #endregion

    #region Assets resolver

    private ObservableCollection<AssetEntry> _minecraftAssets;

    public ObservableCollection<AssetEntry> MinecraftAssets
    {
        get => _minecraftAssets;
        set => this.RaiseAndSetIfChanged(ref _minecraftAssets, value);
    }

    public ReactiveCommand<Unit, Unit> RefreshAssetsCommand { get; set; }

    private void InitializeAssetsResolverCommands()
    {
        RefreshAssetsCommand = ReactiveCommand.Create((RefreshAssets));
    }

    private async void RefreshAssets()
    {
        await Task.Run(async () =>
        {
            if (SelectedMinecraft == null)
            {
                return;
            }

            //GetAssetsAsync will automatically download missing asset index file
            var assets = await SelectedMinecraft.GetAssetsAsync();

            MinecraftAssets = new(assets);
        });
    }

    #endregion

    #region Downloader

    private ObservableCollection<RemoteMinecraftEntry> _remoteMinecrafts = new();

    public ObservableCollection<RemoteMinecraftEntry> RemoteMinecrafts
    {
        get => _remoteMinecrafts;
        set => this.RaiseAndSetIfChanged(ref _remoteMinecrafts, value);
    }

    private bool _releaseSelected;

    public bool ReleaseSelected
    {
        get => _releaseSelected;
        set
        {
            this.RaiseAndSetIfChanged(ref _releaseSelected, value);
            FetchOrRefreshRemoteMinecrafts();
        }
    }

    private bool _snapshotSelected;

    public bool SnapshotSelected
    {
        get => _snapshotSelected;
        set
        {
            this.RaiseAndSetIfChanged(ref _snapshotSelected, value);
            FetchOrRefreshRemoteMinecrafts();
        }
    }

    private bool _ancientSelected;

    public bool AncientSelected
    {
        get => _ancientSelected;
        set
        {
            this.RaiseAndSetIfChanged(ref _ancientSelected, value);
            FetchOrRefreshRemoteMinecrafts();
        }
    }

    private bool _isDownloadingMinecraft;

    public bool IsDownloadingMinecraft
    {
        get => _isDownloadingMinecraft;
        set => this.RaiseAndSetIfChanged(ref _isDownloadingMinecraft, value);
    }

    private string _selectedDownloadSource;

    public string SelectedDownloadSource
    {
        get => _selectedDownloadSource;
        set => this.RaiseAndSetIfChanged(ref _selectedDownloadSource, value);
    }

    private RemoteMinecraftEntry? _selectedRemoteMinecraft;

    public RemoteMinecraftEntry? SelectedRemoteMinecraft
    {
        get => _selectedRemoteMinecraft;
        set => this.RaiseAndSetIfChanged(ref _selectedRemoteMinecraft, value);
    }
    
    public ReactiveCommand<Unit, Unit> FetchRemoteMinecraftsCommand { get; set; }

    public ReactiveCommand<Unit, Unit> DownloadMinecraftCommand { get; set; }

    public ReactiveCommand<Unit, Unit> DownloadLibrariesCommand { get; set; }

    public ReactiveCommand<Unit, Unit> DownloadAssetsCommand { get; set; }

    private void InitializeMinecraftDownloadCommands()
    {
        FetchRemoteMinecraftsCommand = ReactiveCommand.Create(FetchOrRefreshRemoteMinecrafts);
        DownloadMinecraftCommand = ReactiveCommand.Create(DownloadMinecraft);
        DownloadLibrariesCommand = ReactiveCommand.Create(DownloadLibraries);
        DownloadAssetsCommand = ReactiveCommand.Create(DownloadAssets);
    }

    private async void DownloadMinecraft()
    {
        IsDownloadingMinecraft = true;
        if (SelectedRemoteMinecraft != null && DataBus.MinecraftResolver != null)
        {
            try
            {
                var localMinecraft = await SelectedRemoteMinecraft.ResolveLocalEntryAsync(DataBus.MinecraftResolver);
                if (localMinecraft.ValidateChecksum())
                {
                    await GeneralUtils.PromptDialogAsync("Already downloaded");
                    IsDownloadingMinecraft = false;
                    return;
                }
                var downloadUrl = localMinecraft.GetDownloadUrl(SelectedDownloadSource.ResolveDownloadSource());
                await downloadUrl.DownloadFileAsync(localMinecraft.Tree.VersionRoot.FullName,
                    localMinecraft.Tree.Jar.Name);

            }
            catch (Exception e)
            {
                await GeneralUtils.PromptExceptionDialogAsync(e);
            }
        }
        else
        {
            await GeneralUtils.PromptDialogAsync("Please select a Minecraft to download");
        }

        IsDownloadingMinecraft = false;
    }

    private async void FetchOrRefreshRemoteMinecrafts()
    {
        RemoteMinecrafts = new();
        var entries = await MinecraftUtils.GetRemoteMinecraftsAsync();
        if (ReleaseSelected)
        {
            RemoteMinecrafts.Add(entries.Filter(MinecraftJsonType.Release));
        }

        if (SnapshotSelected)
        {
            RemoteMinecrafts.Add(entries.Filter(MinecraftJsonType.Snapshot));
        }

        if (AncientSelected)
        {
            RemoteMinecrafts.Add(entries.Filter(MinecraftJsonType.OldAlpha | MinecraftJsonType.OldBeta));
        }
    }

    private bool _isDownloadingLibraries;

    public bool IsDownloadingLibraries
    {
        get => _isDownloadingLibraries;
        set => this.RaiseAndSetIfChanged(ref _isDownloadingLibraries, value);
    }
    
    private async void DownloadLibraries()
    {
        if (MinecraftLibraries.Count == 0)
        {
            return;
        }

        if (MinecraftLibraries.All(e => e.ValidateChecksum()))
        {
            await GeneralUtils.PromptDialogAsync("All file downloaded, no need to download again");
            return;
        }

        IsDownloadingLibraries = true;

        try
        {
            var chunks = MinecraftLibraries.Where(e => !e.ValidateChecksum()).Batch(10);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await Policy
                .Handle<Exception>()
                .RetryAsync(3)
                .ExecuteAsync(async () =>
                {
                    foreach (var libChunk in chunks)
                    {
                        await Parallel.ForEachAsync(libChunk, async (entry, _) =>
                        {
                            var url = entry.GetDownloadUrl(SelectedDownloadSource.ResolveDownloadSource());
                            if (!url.IsNullOrEmpty())
                            {
                                await url.DownloadFileAsync(entry.File.DirectoryName, entry.File.Name, cancellationToken: _);
                            }
                        });
                    }
                });
            
            stopwatch.Stop();

            await GeneralUtils.PromptDialogAsync($"Download completed in {stopwatch.Elapsed.TotalSeconds}s");
        }
        catch (Exception e)
        {
            await GeneralUtils.PromptExceptionDialogAsync(e);
        }

        IsDownloadingLibraries = false;
    }

    private bool _isDownloadingAssets;

    public bool IsDownloadingAssets
    {
        get => _isDownloadingAssets;
        set => this.RaiseAndSetIfChanged(ref _isDownloadingAssets, value);
    }
    
    private async void DownloadAssets()
    {
        if (MinecraftAssets.Count == 0)
        {
            return;
        }

        if (MinecraftAssets.All(e => e.ValidateChecksum()))
        {
            await GeneralUtils.PromptDialogAsync("All file downloaded, no need to download again");
            return;
        }

        IsDownloadingAssets = true;

        try
        {
            var chunks = MinecraftAssets.Where(e => !e.ValidateChecksum()).Batch(30);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await Policy
                .Handle<Exception>()
                .RetryAsync(3)
                .ExecuteAsync(async () =>
                {
                    foreach (var assetChunk in chunks)
                    {
                        await Parallel.ForEachAsync(assetChunk, async (entry, _) =>
                        {
                            var url = entry.GetDownloadUrl(SelectedDownloadSource.ResolveDownloadSource());
                            if (!url.IsNullOrEmpty())
                            {
                                await url.DownloadFileAsync(entry.File.DirectoryName, entry.File.Name, cancellationToken: _);
                            }
                        });
                    }
                });
            
            stopwatch.Stop();

            await GeneralUtils.PromptDialogAsync($"Download completed in {stopwatch.Elapsed.TotalSeconds}s");
        }
        catch (Exception e)
        {
            await GeneralUtils.PromptExceptionDialogAsync(e);
        }

        IsDownloadingAssets = false;
    }

    #endregion
    
    public ResolverVM()
    {
        InitializeMinecraftResolverCommands();
        InitializeLibrariesResolverCommands();
        InitializeAssetsResolverCommands();
        InitializeMinecraftDownloadCommands();
    }
}