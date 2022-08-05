using Manganese.Text;
using ModuleLauncher.NET.Example.Utils;

namespace ModuleLauncher.NET.Example.ViewModels;

public class SettingsVM : ViewModelBase
{
    private string? _minecraftRootPath;

    public string? MinecraftRootPath
    {
        get => _minecraftRootPath;
        set
        {
            this.RaiseAndSetIfChanged(ref _minecraftRootPath, value);
            DataBus.MinecraftRootPath = value;
        }
    }

    private string? _minecraftWorkingDirectory;

    public string? MinecraftWorkingDirectory
    {
        get => _minecraftWorkingDirectory;
        set
        {
            this.RaiseAndSetIfChanged(ref _minecraftWorkingDirectory, value);
            DataBus.MinecraftWorkingPath = value;
        }
    }

    public ReactiveCommand<Unit, Unit> BrowseMinecraftWorkingDir { get; set; }

    public ReactiveCommand<Unit, Unit> BrowseMinecraftRootPath { get; set; }

    public SettingsVM()
    {
        BrowseMinecraftRootPath = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await GeneralUtils.DirectoryBrowser("Select a .minecraft directory");
            if (result.IsNullOrEmpty())
            {
                return;
            }

            MinecraftRootPath = result;
        });

        BrowseMinecraftWorkingDir = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await GeneralUtils.DirectoryBrowser("Select a working path");
            if (result.IsNullOrEmpty())
            {
                return;
            }

            MinecraftWorkingDirectory = result;
        });
    }
}