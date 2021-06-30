using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.Example.ViewModels.Locators;

namespace ModuleLauncher.Example.Views.Locators
{
    public class MinecraftLocatorView : UserControl
    {
        public MinecraftLocatorView()
        {
            InitializeComponent();

            DataContext = new MinecraftLocatorViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}