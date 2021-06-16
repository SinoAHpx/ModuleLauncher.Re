using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Example.ViewModels.Authenticators;

namespace ModuleLauncher.Example.Views.Authenticators
{
    public partial class MicrosoftAuthenticatorWebBrowser : Window
    {
        public MicrosoftAuthenticatorWebBrowser()
        {
            InitializeComponent();

#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public string Code { get; set; }

        private async void BaseCefBrowser_OnAddressChanged(object sender, string address)
        {
            await Dispatcher.UIThread.InvokeAsync((async () =>
            {
                if (address.Contains("https://login.live.com/oauth20_desktop.srf"))
                {
                    Code = address.Replace("https://login.live.com/oauth20_desktop.srf?code=", "");

                    Code = Code.Replace(Code[Code.IndexOf("&lc=", StringComparison.Ordinal)..], "");

                    Console.WriteLine(Code);
                    Close();
                }
            }));
        }
    }
}
