using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.NET.Example.ViewModels;

namespace ModuleLauncher.NET.Example.Views;

public partial class LauncherView : UserControl
{
    public LauncherView()
    {
        InitializeComponent();

        DataContext = new LauncherVM();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}