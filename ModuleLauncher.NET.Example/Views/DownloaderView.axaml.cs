using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.NET.Example.ViewModels;

namespace ModuleLauncher.NET.Example.Views;

public partial class DownloaderView : UserControl
{
    public DownloaderView()
    {
        InitializeComponent();
        DataContext = new DownloaderVM();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}