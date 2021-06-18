using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Utils.Extensions;
using ReactiveUI;

namespace ModuleLauncher.Example.ViewModels.Authenticators
{
    public class OfflineAuthenticatorViewModel : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        
        public async void Authenticate()
        {
            await MessageBoxEx.Show((await new OfflineAuthenticator(Name).Authenticate()).ToJsonString());
        }
    }
}