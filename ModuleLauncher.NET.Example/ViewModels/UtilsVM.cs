using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Example.Utils;
using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Example.ViewModels;

public class UtilsVM : ViewModelBase
{
    #region Java version grabber

    private string? _javaExePath;

    public string? JavaExePath
    {
        get => _javaExePath;
        set => this.RaiseAndSetIfChanged(ref _javaExePath, value);
    }

    public ReactiveCommand<Unit, Unit> BrowseJavaCommand { get; set; }

    public ReactiveCommand<Unit, Unit> GetJavaVersionCommand { get; set; }

    private void InitializeJavaVersionGrabberCommands()
    {
        BrowseJavaCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await GeneralUtils.OpenFileBrowserAsync("Select java runtime");
            if (result.IsNullOrEmpty())
            {
                return;
            }

            JavaExePath = result;
        });

        GetJavaVersionCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (JavaExePath.IsNullOrEmpty())
            {
                return;
            }

            var version = new FileInfo(JavaExePath).GetJavaExecutableVersion();
            if (version != null)
            {
                await GeneralUtils.PromptDialogAsync($"Java version is: {version}");
            }
            else
            {
                await GeneralUtils.PromptDialogAsync($"Cannot get java version");
            }
        });
    }

    #endregion

    #region Sha1 grabber

    private string? _toGetSha1;

    public string? ToGetSha1
    {
        get => _toGetSha1;
        set => this.RaiseAndSetIfChanged(ref _toGetSha1, value);
    }

    public ReactiveCommand<Unit, Unit> BrowseToGetSha1Command { get; set; }

    public ReactiveCommand<Unit, Unit> GetSha1Command { get; set; }

    private void InitializeSha1GrabberCommands()
    {
        BrowseToGetSha1Command = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await GeneralUtils.OpenFileBrowserAsync("Select sha1", new List<FileDialogFilter>());
            if (result.IsNullOrEmpty())
            {
                return;
            }

            ToGetSha1 = result;
        });

        GetSha1Command = ReactiveCommand.CreateFromTask( async () =>
        {
            if (ToGetSha1.IsNullOrEmpty())
            {
                return;
            }
            await GeneralUtils.PromptDialogAsync(
                $"Sha1 of {ToGetSha1.GetFileName()}: {new FileInfo(ToGetSha1).GetSha1()}");
        });
    }

    #endregion

    public UtilsVM()
    {
        InitializeJavaVersionGrabberCommands();
        InitializeSha1GrabberCommands();
    }
}