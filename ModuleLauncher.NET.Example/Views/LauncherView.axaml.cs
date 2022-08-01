using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ModuleLauncher.NET.Example.Views;

public partial class LauncherView : UserControl
{
    public LauncherView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}