using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Example.Utils;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Example.ViewModels;

public class LauncherVM : ViewModelBase
{
    #region Minecraft versions selector

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
        set { this.RaiseAndSetIfChanged(ref _selectedMinecraft, value); }
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
                await GeneralUtils.Exception(e);
            }
        }
        else
        {
            await GeneralUtils.Dialog("No versions could be found, have you set your .minecraft path?");
        }
    }

    private ObservableCollection<MinecraftJava> _minecraftJavas = new();

    public ObservableCollection<MinecraftJava> MinecraftJavas
    {
        get => _minecraftJavas;
        set => this.RaiseAndSetIfChanged(ref _minecraftJavas, value);
    }

    private MinecraftJava? _selectedJava;

    public MinecraftJava? SelectedJava
    {
        get => _selectedJava;
        set => this.RaiseAndSetIfChanged(ref _selectedJava, value);
    }

    public ReactiveCommand<Unit, Unit> RemoveSelectedJavaCommand { get; set; }

    public ReactiveCommand<Unit, Unit> BrowseJavaCommand { get; set; }

    private void InitializeMinecraftSelectorCommands()
    {
        RefreshMinecraftVersionsCommand = ReactiveCommand.Create(RefreshMinecraftVersions);
        RemoveSelectedJavaCommand = ReactiveCommand.Create(RemoveSelectedJava);
        BrowseJavaCommand = ReactiveCommand.Create((BrowseJava));
    }

    private async void BrowseJava()
    {
        var result = await GeneralUtils.FileBrowser("Select java runtime");
        if (result.IsNullOrEmpty())
        {
            return;
        }

        var javaVersion = new FileInfo(result).GetJavaExecutableVersion();
        if (javaVersion == null)
        {
            var dialogResult = await GeneralUtils.Input("Java version: ");
            if (int.TryParse(dialogResult, out var parsedVersion))
            {
                javaVersion = parsedVersion;
            }
        }

        MinecraftJavas.Add(new MinecraftJava
        {
            Executable = new FileInfo(result),
            Version = javaVersion!.Value
        });
    }

    private void RemoveSelectedJava()
    {
        if (SelectedJava != null)
        {
            MinecraftJavas.Remove(SelectedJava);
        }
    }

    #endregion

    #region Launch config

    private int? _windowWidth;

    public int? WindowWidth
    {
        get => _windowWidth;
        set
        {
            this.RaiseAndSetIfChanged(ref _windowWidth, value);
            _launcher.LauncherConfig.WindowWidth = value;
        }
    }

    private int? _windowHeight;

    public int? WindowHeight
    {
        get => _windowHeight;
        set
        {
            this.RaiseAndSetIfChanged(ref _windowHeight, value);
            _launcher.LauncherConfig.WindowHeight = value;
        }
    }

    private bool _isFullscreen;

    public bool IsFullscreen
    {
        get => _isFullscreen;
        set
        {
            this.RaiseAndSetIfChanged(ref _isFullscreen, value);
            _launcher.LauncherConfig.Fullscreen = value;
        }
    }

    private int? _maxMemorySize;

    public int? MaxMemorySize
    {
        get => _maxMemorySize;
        set
        {
            this.RaiseAndSetIfChanged(ref _maxMemorySize, value);
            if (value != null)
            {
                _launcher.LauncherConfig.MaxMemorySize = value.Value;
            }
        }
    }

    private int? _minMemorySize;

    public int? MinMemorySize
    {
        get => _minMemorySize;
        set
        {
            this.RaiseAndSetIfChanged(ref _minMemorySize, value);
            _launcher.LauncherConfig.MinMemorySize = value;
        }
    }

    private string? _launcherName;

    public string? LauncherName
    {
        get => _launcherName;
        set
        {
            this.RaiseAndSetIfChanged(ref _launcherName, value);
            if (!value.IsNullOrEmpty())
            {
                _launcher.LauncherConfig.LauncherName = value;
            }
        }
    }

    private string? _authenticationName;

    public string? AuthenticationName
    {
        get => _authenticationName;
        set
        {
            this.RaiseAndSetIfChanged(ref _authenticationName, value);
            if (!value.IsNullOrEmpty())
            {
                _launcher.LauncherConfig.Authentication = value;
            }
        }
    }

    private string? _launchArguments;

    public string? LaunchArguments
    {
        get => _launchArguments;
        set => this.RaiseAndSetIfChanged(ref _launchArguments, value);
    }

    private string? _launchOutputs;

    public string? LaunchOutputs
    {
        get => _launchOutputs;
        set => this.RaiseAndSetIfChanged(ref _launchOutputs, value);
    }
    
    public ReactiveCommand<Unit, Unit> GenerateLaunchArgumentsCommand { get; set; }

    public ReactiveCommand<Unit, Unit> LaunchCommand { get; set; }

    private void InitializeLaunchCommands()
    {
        GenerateLaunchArgumentsCommand = ReactiveCommand.Create(GenerateLaunchArguments);
        LaunchCommand = ReactiveCommand.Create(Launch);
    }

    private readonly Launcher _launcher = new();

    private async void Launch()
    {
        if (SelectedMinecraft == null)
        {
            return;
        }

        if (AuthenticationName.IsNullOrEmpty())
        {
            AuthenticationName = "SgtPepper";
        }

        if (DataBus.AuthenticateResult != null)
        {
            _launcher.LauncherConfig.Authentication = DataBus.AuthenticateResult;
        }

        await Task.Run(async () =>
        {
            var process = await _launcher.LaunchAsync(SelectedMinecraft);
            while (!(await process.ReadOutputLineAsync()).IsNullOrEmpty())
            {
                LaunchOutputs += $"{await process.ReadOutputLineAsync()}\r\n";
            }
        });
    }

    private async void GenerateLaunchArguments()
    {
        if (SelectedMinecraft == null)
        {
            return;
        }

        await Task.Run(() => { LaunchArguments = _launcher.GetLaunchArguments(SelectedMinecraft); });
    }

    #endregion

    public LauncherVM()
    {
        InitializeMinecraftSelectorCommands();
        InitializeLaunchCommands();

        this.WhenAnyValue(x => x.MinecraftJavas.Count)
            .Subscribe(value =>
            {
                _launcher.LauncherConfig.Javas = new List<MinecraftJava>(MinecraftJavas);
            });
    }
}