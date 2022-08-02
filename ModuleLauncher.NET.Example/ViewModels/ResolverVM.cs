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
        set => this.RaiseAndSetIfChanged(ref _selectedMinecraft, value);
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
                    await GeneralUtils.PromptExceptionDialog(e);
                }
            }
            else
            {
                await GeneralUtils.PromptDialog("No versions could be found, have you set your .minecraft path?");
            }
        });
    }


    public ResolverVM()
    {
        RefreshMinecraftVersionsCommand = ReactiveCommand.CreateFromTask(RefreshMinecraftVersions);
        UpdateMinecraftTreeCommand = ReactiveCommand.Create(UpdateMinecraftTree);
    }


    private ObservableCollection<MinecraftTreeNode> _minecraftTreeItems;

    public ObservableCollection<MinecraftTreeNode> MinecraftTreeItems
    {
        get => _minecraftTreeItems;
        set => this.RaiseAndSetIfChanged(ref _minecraftTreeItems, value);
    }

    public ReactiveCommand<Unit, Unit> UpdateMinecraftTreeCommand { get; set; }

    private void UpdateMinecraftTree()
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
}