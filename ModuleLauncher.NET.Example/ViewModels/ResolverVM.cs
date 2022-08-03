using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DynamicData;
using ModuleLauncher.NET.Example.Utils;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;

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

    private async Task RefreshMinecraftVersions()
    {
        await Task.Run(async () =>
        {
            if (DataBus.MinecraftResolver != null)
            {
                try
                {
                    var minecrafts = DataBus.MinecraftResolver.GetMinecrafts();

                    //minecraftEntry.ValidateChecksum means check if a minecraft entry is valid
                    MinecraftVersions = new ObservableCollection<MinecraftEntry>(minecrafts);
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
        });
    }

    private void InitializeMinecraftResolverCommands()
    {
        RefreshMinecraftVersionsCommand = ReactiveCommand.CreateFromTask(RefreshMinecraftVersions);
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
    
    public ResolverVM()
    {
        InitializeMinecraftResolverCommands();
        InitializeLibrariesResolverCommands();
        InitializeAssetsResolverCommands();
    }
}