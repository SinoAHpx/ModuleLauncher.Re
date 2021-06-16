using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.Example.ViewModels.Authenticators;

namespace ModuleLauncher.Example.Views.Authenticators
{
    public partial class MicrosoftAuthenticatorView : UserControl
    {
        public MicrosoftAuthenticatorView()
        {
            InitializeComponent();

            DataContext = new MicrosoftAuthenticatorViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
