using System;
using System.Diagnostics;
using System.Reactive;
using System.Runtime.InteropServices;
using Manganese.Text;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Example.Utils;
using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Example.ViewModels;

public class AuthVM : ViewModelBase
{
    #region Microsoft properties

    private string? _microsoftRedirectUrl;
    public string? MicrosoftRedirectUrl
    {
        get => _microsoftRedirectUrl;
        set
        {
            if (!value.IsNullOrEmpty())
            {
                _microsoftAuthenticator.RedirectUrl = value;
            }
            this.RaiseAndSetIfChanged(ref _microsoftRedirectUrl, value);
        }
    }

    private string? _microsoftClientId;

    public string? MicrosoftClientId
    {
        get => _microsoftClientId;
        set
        {
            if (!value.IsNullOrEmpty())
            {
                _microsoftAuthenticator.ClientId = value;
            }
            this.RaiseAndSetIfChanged(ref _microsoftClientId, value);
        }
    }
    

    private string? _microsoftAccessToken;

    public string? MicrosoftAccessToken
    {
        get => _microsoftAccessToken;
        set => this.RaiseAndSetIfChanged(ref _microsoftAccessToken, value);
    }

    private string? _microsoftUsername;

    public string? MicrosoftUsername
    {
        get => _microsoftUsername;
        set => this.RaiseAndSetIfChanged(ref _microsoftUsername, value);
    }

    private string? _microsoftUUID;

    public string? MicrosoftUUID
    {
        get => _microsoftUUID;
        set => this.RaiseAndSetIfChanged(ref _microsoftUUID, value);
    }

    private string? _microsoftRefreshToken;

    public string? MicrosoftRefreshToken
    {
        get => _microsoftRefreshToken;
        set => this.RaiseAndSetIfChanged(ref _microsoftRefreshToken, value);
    }

    private string? _microsoftExpireIn;

    public string? MicrosoftExpireIn
    {
        get => _microsoftExpireIn;
        set => this.RaiseAndSetIfChanged(ref _microsoftExpireIn, value);
    }

    private MicrosoftAuthenticator _microsoftAuthenticator = new();
    
    public string? MicrosoftAuthenticateUrl
    {
        get => _microsoftAuthenticator.LoginUrl;
    }

    private string? _isMicrosoftAuthenticated;

    public string? IsMicrosoftAuthenticated
    {
        get => _isMicrosoftAuthenticated;
        set => this.RaiseAndSetIfChanged(ref _isMicrosoftAuthenticated, value);
    }

    private bool _openedBrowser;

    public bool OpenedBrowser
    {
        get => _openedBrowser;
        set => this.RaiseAndSetIfChanged(ref _openedBrowser, value);
    }

    private string? _microsoftRedirectedUrl;

    public string? MicrosoftRedirectedUrl
    {
        get => _microsoftRedirectedUrl;
        set => this.RaiseAndSetIfChanged(ref _microsoftRedirectedUrl, value);
    }
    
    public ReactiveCommand<Unit, Unit> MicrosoftOpenBrowserCommand { get; set; }
    public ReactiveCommand<Unit, Unit> MicrosoftAuthenticateCommand { get; set; }

    public ReactiveCommand<Unit, Unit> MicrosoftRefreshCommand { get; set; }

    #endregion
    

    #region Offline properties

    private string? _offlineUsername;

    public string? OfflineUsername
    {
        get => _offlineUsername;
        set => this.RaiseAndSetIfChanged(ref _offlineUsername, value);
    }

    private string? _offlineAccessToken;

    public string? OfflineAccessToken
    {
        get => _offlineAccessToken;
        set => this.RaiseAndSetIfChanged(ref _offlineAccessToken, value);
    }

    private string? _offlineUUID;

    public string? OfflineUUID
    {
        get => _offlineUUID;
        set => this.RaiseAndSetIfChanged(ref _offlineUUID, value);
    }

    private bool _isOfflineAuthenticated;

    public bool IsOfflineAuthenticated
    {
        get => _isOfflineAuthenticated; 
        set => this.RaiseAndSetIfChanged(ref _isOfflineAuthenticated, value);
    }
    
    public ReactiveCommand<Unit, Unit> OfflineAuthenticateCommand { get; set; }

    #endregion
    
    public AuthVM()
    {
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        OfflineAuthenticateCommand = ReactiveCommand.Create(OfflineAuthenticate);
        MicrosoftOpenBrowserCommand = ReactiveCommand.Create(MicrosoftOpenBrowser);
        MicrosoftAuthenticateCommand = ReactiveCommand.Create(MicrosoftAuthenticate);
        MicrosoftRefreshCommand = ReactiveCommand.Create(MicrosoftRefresh);
    }

    private async void MicrosoftRefresh()
    {
        if (MicrosoftRefreshToken.IsNullOrEmpty()) return;

        try
        {
            //do authenticate
            var result = await _microsoftAuthenticator.RefreshAuthenticateAsync(MicrosoftRefreshToken);
            
            //assign value
            MicrosoftAccessToken = result.AccessToken;
            MicrosoftUUID = result.UUID;
            MicrosoftUsername = result.Name;
            MicrosoftRefreshToken = result.RefreshToken;
            MicrosoftExpireIn = result.ExpireIn.ToString("g");
            
            //assign global value, so that other views can share this result
            DataBus.AuthenticateResult = result;
        }
        catch (Exception e)
        {
            await GeneralUtils.PromptExceptionDialogAsync(e);
        }
    }

    private async void MicrosoftAuthenticate()
    {
        if (MicrosoftRedirectedUrl.IsNullOrEmpty()) return;
        try
        {
            //extract "code" parameter from url
            var code = MicrosoftRedirectedUrl.ExtractCode();
            _microsoftAuthenticator.Code = code;
            
            //execute authenticate
            var result = await _microsoftAuthenticator.AuthenticateAsync();
            
            //assign value
            MicrosoftAccessToken = result.AccessToken;
            MicrosoftUUID = result.UUID;
            MicrosoftUsername = result.Name;
            MicrosoftRefreshToken = result.RefreshToken;
            MicrosoftExpireIn = result.ExpireIn.ToString("g");
            
            //assign global value, so that other views can share this result
            DataBus.AuthenticateResult = result;
        }
        catch (Exception e)
        {
            await GeneralUtils.PromptExceptionDialogAsync(e);
        }
        
    }

    private void MicrosoftOpenBrowser()
    {
        if (!MicrosoftAuthenticateUrl.IsNullOrEmpty())
        {
            OpenedBrowser = true;
            GeneralUtils.OpenUrl(MicrosoftAuthenticateUrl);
        }
    }

    private void OfflineAuthenticate()
    {
        if (!OfflineUsername.IsNullOrEmpty())
        {
            IsOfflineAuthenticated = true;
            var authenticateResult = new OfflineAuthenticator(OfflineUsername).Authenticate();
            OfflineAccessToken = authenticateResult.AccessToken;
            OfflineUUID = authenticateResult.UUID;
        }
    }
}