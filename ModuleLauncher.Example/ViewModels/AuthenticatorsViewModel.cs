using System;
using System.Reactive;
using Avalonia.Animation.Easings;
using MessageBox.Avalonia;
using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Example.Views;
using ModuleLauncher.Re.Authenticators;
using ModuleLauncher.Re.Models.Authenticators;
using ModuleLauncher.Re.Utils.Extensions;
using ReactiveUI;

namespace ModuleLauncher.Example.ViewModels
{
    public class AuthenticatorsViewModel : ViewModelBase
    {
        private string _account;

        public string Account
        {
            get => _account;
            set => this.RaiseAndSetIfChanged(ref _account, value);
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        private string _clientToken;

        public string ClientToken
        {
            get => _clientToken;
            set => this.RaiseAndSetIfChanged(ref _clientToken, value);
        }

        private string _titleColor = "DarkRed";

        public string TitleColor
        {
            get => _titleColor;
            set => this.RaiseAndSetIfChanged(ref _titleColor, value);
        }

        public AuthenticatorsViewModel()
        {
            
        }

        public void GetRandomClientToken()
        {
            ClientToken = Guid.NewGuid().ToString("N");
        }

        private AuthenticateResult _authenticateResult;

        public async void Authenticate()
        {
            try
            {
                var ma = new MojangAuthenticator
                {
                    Account = Account,
                    Password = Password,
                    ClientToken = ClientToken
                };

                _authenticateResult = await ma.Authenticate();

                await MessageBoxEx.Show(_authenticateResult.ToJsonString());

                TitleColor = "DarkGreen";
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.ToString());

                TitleColor = "DarkRed";
            }
        }

        public async void Refresh()
        {
            try
            {
                _authenticateResult = await _authenticateResult.Refresh();

                //or you can use:
                //
                // var ma = new MojangAuthenticator
                //  {
                //      Account = Account,
                //      Password = Password,
                //      ClientToken = ClientToken
                //  };
                // await ma.Refresh(_authenticateResult.AccessToken, _authenticateResult.ClientToken);
                //
                //same below

                await MessageBoxEx.Show(_authenticateResult.ToJsonString());
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.ToString());

                TitleColor = "DarkRed";
            }
        }

        public async void Validate()
        {
            try
            {
                var re = await _authenticateResult.Validate();

                await MessageBoxEx.Show($"Validation of {_authenticateResult.AccessToken}:\r\n{re}");

                TitleColor = re ? "DarkGreen" : "DarkRed";
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.ToString());

                TitleColor = "DarkRed";
            }
        }

        public async void Invalidate()
        {
            try
            {
                await _authenticateResult.Invalidate();

                await MessageBoxEx.Show($"Validation of {_authenticateResult.AccessToken}:\r\n{await _authenticateResult.Validate()}");

                TitleColor = "DarkRed";
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.ToString());

                TitleColor = "DarkRed";
            }
        }

        public async void SignOut()
        {
            try
            {
                var ma = new MojangAuthenticator
                {
                    Account = Account,
                    Password = Password,
                    ClientToken = ClientToken
                };

                await ma.SignOut();

                await MessageBoxEx.Show($"Validation of {_authenticateResult.AccessToken}:\r\n{await _authenticateResult.Validate()}");

                TitleColor = "DarkRed";
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.ToString());

                TitleColor = "DarkRed";
            }
        }
    }
}