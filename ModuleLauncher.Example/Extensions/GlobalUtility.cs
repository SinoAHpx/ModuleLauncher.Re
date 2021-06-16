using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ModuleLauncher.Example.Views;

namespace ModuleLauncher.Example.Extensions
{
    public static class GlobalUtility
    {
        public static Window GetMainWindow()
        {
            return (Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime).MainWindow;
        }
    }
}