using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ModuleLauncher.NET.Example.ViewModels;

namespace ModuleLauncher.NET.Example.Views;

public partial class ResolverView : UserControl
{
    public ResolverView()
    {
        InitializeComponent();

        DataContext = new ResolverVM();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}