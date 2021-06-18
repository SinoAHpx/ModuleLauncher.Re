using System;
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

        private string _token;
        private MicrosoftAuthenticator _authenticator;
        
        public async void Authenticate()
        {
            _authenticator = new(Code);

            try
            {
                var re = await _authenticator.Authenticate();

                _token = re.AccessToken;

                await MessageBoxEx.Show(re.ToJsonString());
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.ToString());
            }
        }

        public async void CheckOwnership()
        {
            try
            {
                var hasMinecraft = await _authenticator.CheckMinecraftOwnership(_token);
                await MessageBoxEx.Show($"Your account has {(hasMinecraft ? "" : "no ")}minecraft");
            }
            catch
            {
                await MessageBoxEx.Show("Authenticate first");
            }
        }

        private string _code;

        public string Code
        {
            get => _code;
            set => this.RaiseAndSetIfChanged(ref _code, value);
        }   
    }
}