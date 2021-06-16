using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.Example.ViewModels.Authenticators;

namespace ModuleLauncher.Example.Views.Authenticators
{
    public partial class MojangAuthenticatorView : UserControl
    {
        public MojangAuthenticatorView()
        {
            InitializeComponent();

            DataContext = new MojangAuthenticatorViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
