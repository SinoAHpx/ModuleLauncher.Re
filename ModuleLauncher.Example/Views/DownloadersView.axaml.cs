using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.Example.ViewModels.Downloaders;

namespace ModuleLauncher.Example.Views
{
    public partial class DownloadersView : UserControl
    {
        public DownloadersView()
        {
            InitializeComponent();

            DataContext = new DownloadersViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
