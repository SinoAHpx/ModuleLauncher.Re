using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.NET.Example.ViewModels;

namespace ModuleLauncher.NET.Example.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        DataContext = new SettingsVM();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}