using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.Example.ViewModels.Launcher;

namespace ModuleLauncher.Example.Views
{
    public partial class LauncherView : UserControl
    {
        public LauncherView()
        {
            InitializeComponent();

            DataContext = new LauncherViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
