using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.NET.Example.ViewModels;

namespace ModuleLauncher.NET.Example.Views;

public partial class UtilsView : UserControl
{
    public UtilsView()
    {
        InitializeComponent();

        DataContext = new UtilsVM();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}