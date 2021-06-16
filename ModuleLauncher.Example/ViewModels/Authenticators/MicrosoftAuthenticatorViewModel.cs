using Avalonia.Controls;
using Avalonia.Metadata;
using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Example.Views.Authenticators;
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

        private string _code;

        public string Code
        {
            get => _code;
            set => this.RaiseAndSetIfChanged(ref _code, value);
        }   
    }
}