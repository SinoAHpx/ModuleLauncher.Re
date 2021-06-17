using Avalonia.Controls;
using Avalonia.Metadata;
using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Example.Views.Authenticators;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Utils.Extensions;
using ReactiveUI;

namespace ModuleLauncher.Example.ViewModels.Authenticators
{
    public class MicrosoftAuthenticatorViewModel : ViewModelBase
    {
        public async void ShowWebBrowser()
        {
            var window = new MicrosoftAuthenticatorWebBrowser
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            
            await window.ShowDialog(GlobalUtility.GetMainWindow());

            Code = window.Code;
        }

        private MicrosoftAuthenticator _authenticator;
        public async void Authenticate()
        {
            _authenticator = new MicrosoftAuthenticator(Code);

            var re = await _authenticator.GetMicrosoftAuthorizeToken();

            var de = await _authenticator.AuthenticateXboxLive(re);

            await MessageBoxEx.Show(de.ToJsonString());
        }

        public void CheckOwnership()
        {

        }

        private string _code;

        public string Code
        {
            get => _code;
            set => this.RaiseAndSetIfChanged(ref _code, value);
        }   
    }
}