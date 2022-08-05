using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var result = await GeneralUtils.FileBrowser("Select java runtime");
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
                await GeneralUtils.Dialog($"Java version is: {version}");
            }
            else
            {
                await GeneralUtils.Dialog($"Cannot get java version");
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
            var result = await GeneralUtils.FileBrowser("Select sha1", new List<FileDialogFilter>());
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
            await GeneralUtils.Dialog(
                $"Sha1 of {ToGetSha1.GetFileName()}: {new FileInfo(ToGetSha1).GetSha1()}");
        });
    }

    #endregion

    private string? _accessToken = DataBus.AuthenticateResult?.AccessToken;

    public string? AccessToken
    {
        get => _accessToken;
        set => this.RaiseAndSetIfChanged(ref _accessToken, value);
    }

    #region Skin changer

    private string? _skinFile;

    public string? SkinFile
    {
        get => _skinFile;
        set => this.RaiseAndSetIfChanged(ref _skinFile, value);
    }

    public ReactiveCommand<Unit, Unit> BrowseSkinFileCommand { get; set; }

    public ReactiveCommand<Unit, Unit> ChangeSkinByFileCommand { get; set; }

    private string? _skinFileUrl;

    public string? SkinFileUrl
    {
        get => _skinFileUrl;
        set => this.RaiseAndSetIfChanged(ref _skinFileUrl, value);
    }

    public ReactiveCommand<Unit, Unit> ChangeSkinByUrlCommand { get; set; }

    public ReactiveCommand<Unit, Unit> ResetSkinCommand { get; set; }

    public ReactiveCommand<Unit, Unit> HideCapeCommand { get; set; }

    public ReactiveCommand<Unit, Unit> ShowCapeCommand { get; set; }

    private void InitializeSkinChangerCommands()
    {
        BrowseSkinFileCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await GeneralUtils.FileBrowser("Browse skin file", new List<FileDialogFilter>
            {
                new()
                {
                    Extensions = new() { "jpg", "png" }
                }
            });

            if (!result.IsNullOrEmpty())
            {
                SkinFile = result;
            }
        });

        ChangeSkinByFileCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (SkinFile.IsNullOrEmpty() || AccessToken.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Access Token and skin file is required");
                return;
            }

            try
            {
                await SkinUtils.ChangeSkinAsync(AccessToken, new FileInfo(SkinFile));

            }
            catch (Exception e)
            {
                await GeneralUtils.Exception(e);
            }
        });

        ChangeSkinByUrlCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (SkinFileUrl.IsNullOrEmpty() || AccessToken.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Access Token and skin file is required");
                return;
            }

            try
            {
                await SkinUtils.ChangeSkinAsync(AccessToken, SkinFileUrl);
            }
            catch (Exception e)
            {
                await GeneralUtils.Exception(e);
            }
        });
        
        ResetSkinCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (AccessToken.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Access Token is required");
                return;
            }

            await SkinUtils.ResetSkinAsync(AccessToken);
        });
        
        HideCapeCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (AccessToken.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Access Token is required");
                return;
            }

            await SkinUtils.HideCapeAsync(AccessToken);
        });
        
        ShowCapeCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (AccessToken.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Access Token is required");
                return;
            }

            var capeId = await GeneralUtils.Input("Cape id required", "Cape id:");
            if (capeId.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Cape id is required");
                return;
            }

            await SkinUtils.ShowCapeAsync(AccessToken, capeId);
        });
    }

    #endregion

    #region Mojang api

    private string? _username;

    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public ReactiveCommand<Unit, Unit> GetUuidByNameCommand { get; set; }

    private string? _UUID;

    public string? UUID
    {
        get => _UUID;
        set => this.RaiseAndSetIfChanged(ref _UUID, value);
    }

    public ReactiveCommand<Unit, Unit> GetNameHistoryCommand { get; set; }

    public ReactiveCommand<Unit, Unit> GetProfileCommand { get; set; }

    public ReactiveCommand<Unit, Unit> GetNameChangeInfoCommand { get; set; }

    private string? _newUsername;

    public string? NewUsername
    {
        get => _newUsername;
        set => this.RaiseAndSetIfChanged(ref _newUsername, value);
    }

    public ReactiveCommand<Unit, Unit> CheckNameAvailabilityCommand { get; set; }

    public ReactiveCommand<Unit, Unit> ChangeNameCommand { get; set; }

    private void InitializeMojangApiCommands()
    {
        GetUuidByNameCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (Username.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Username is required");
                return;
            }

            try
            {
                UUID = await MojangApiUtils.GetUuidByUsernameAsync(Username);

            }
            catch (Exception e)
            {
                await GeneralUtils.Exception(e);
            }
        });
        
        GetNameHistoryCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (UUID.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("UUID is required");
                return;
            }
            try
            {
                var history = await MojangApiUtils.GetNameHistoryByUuidAsync(UUID);
                await GeneralUtils.Dialog(history.Select(x => $"Name: {x.name}, Changed at: {x.time:s}")
                    .JoinToString("\n"));
            }
            catch (Exception e)
            {
                await GeneralUtils.Exception(e);
            }
        });
        
        GetProfileCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (UUID.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("UUID is required");
                return;
            }
            
            try
            {
                var profile = await MojangApiUtils.GetProfileByUuidAsync(UUID);

                await GeneralUtils.Dialog(profile.ToJsonString());
            }
            catch (Exception e)
            {
                await GeneralUtils.Exception(e);
            }
        });
        
        GetNameChangeInfoCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (AccessToken.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Access Token is required");
                return;
            }

            try
            {
                var info = await MojangApiUtils.GetProfileNameChangeInfoAsync(AccessToken);
                await GeneralUtils.Dialog(info.ToJsonString());
            }
            catch (Exception e)
            {
                await GeneralUtils.Exception(e);
            }
        });
        
        CheckNameAvailabilityCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (AccessToken.IsNullOrEmpty() || NewUsername.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Access token and new username is required");
                return;
            }

            try
            {
                var availability = await MojangApiUtils.CheckNameAvailabilityAsync(AccessToken, NewUsername);
                await GeneralUtils.Dialog(availability ? "Available" : "Unavailable");
            }
            catch (Exception e)
            {
                await GeneralUtils.Exception(e);
            }
        });
        
        ChangeNameCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (AccessToken.IsNullOrEmpty() || NewUsername.IsNullOrEmpty())
            {
                await GeneralUtils.Dialog("Access token and new username is required");
                return;
            }

            try
            {
                var profile = await MojangApiUtils.ChangeUsernameAsync(AccessToken, NewUsername);
                await GeneralUtils.Dialog(profile.ToJsonString());
            }
            catch (Exception e)
            {
                await GeneralUtils.Exception(e);
            }
        });
    }

    #endregion

    public UtilsVM()
    {
        InitializeJavaVersionGrabberCommands();
        InitializeSha1GrabberCommands();
        InitializeSkinChangerCommands();
        InitializeMojangApiCommands();
    }
}