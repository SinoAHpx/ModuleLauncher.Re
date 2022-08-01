using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ModuleLauncher.NET.Example.Views;

public partial class AuthView : UserControl
{
    public AuthView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}