using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.Example.ViewModels;

namespace ModuleLauncher.Example.Views
{
    public partial class AuthenticatorsView : UserControl
    {
        public AuthenticatorsView()
        {
            InitializeComponent();

            DataContext = new AuthenticatorsViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
