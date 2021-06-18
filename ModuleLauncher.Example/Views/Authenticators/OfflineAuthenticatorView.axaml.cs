using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.Example.ViewModels.Authenticators;

namespace ModuleLauncher.Example.Views.Authenticators
{
    public class OfflineAuthenticatorView : UserControl
    {
        public OfflineAuthenticatorView()
        {
            InitializeComponent();

            DataContext = new OfflineAuthenticatorViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}