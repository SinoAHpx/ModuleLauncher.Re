using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ModuleLauncher.NET.Example.Views;

public partial class DownloaderView : UserControl
{
    public DownloaderView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}