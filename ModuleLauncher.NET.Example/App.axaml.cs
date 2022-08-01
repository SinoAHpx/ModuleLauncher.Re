using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ModuleLauncher.NET.Example.ViewModels;
using ModuleLauncher.NET.Example.Views;

namespace ModuleLauncher.NET.Example
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowVM(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}