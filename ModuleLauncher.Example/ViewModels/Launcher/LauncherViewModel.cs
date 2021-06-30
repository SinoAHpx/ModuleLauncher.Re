using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Locators.Minecraft;
using MoreLinq;
using ReactiveUI;

namespace ModuleLauncher.Example.ViewModels.Launcher
{
    public class LauncherViewModel : ViewModelBase
    {
        #region Minecrafts

        public ObservableCollection<Minecraft> Minecrafts { get; set; } = new();
        public async void GetMinecraftList()
        {
            try
            {
                var minecraftLocator = new MinecraftLocator(Root);

                var localMinecrafts = await minecraftLocator.GetLocalMinecrafts();

                localMinecrafts.ForEach(x => Minecrafts.Add(x));
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }
        }

        private Minecraft _selectMc;

        public Minecraft SelectMc
        {
            get => _selectMc;
            set => this.RaiseAndSetIfChanged(ref _selectMc, value);
        }

        #endregion

        #region Launch

        private string _output;

        public string Output
        {
            get => _output;
            set => this.RaiseAndSetIfChanged(ref _output, value);
        }
        public async void Launch()
        {
            var launcher = new Re.Launcher.Launcher(Root)
            {
                Authentication = PlayerName,
                Java = Java,
                Fullscreen = Fullscreen,
                LauncherName = LauncherName,
                MaximumMemorySize = Convert.ToInt32(MaxMemory),
                MinimumMemorySize = Convert.ToInt32(MinMemory),
                WindowHeight = string.IsNullOrEmpty(MinecraftHeight) ? null : Convert.ToInt32(MinecraftHeight),
                WindowWidth = string.IsNullOrEmpty(MinecraftWidth) ? null : Convert.ToInt32(MinecraftWidth)
            };

            var process = await launcher.Launch(SelectMc.Raw.Id);

            process.Exited += (_, _) =>
            {
                Output = $"Process exited with code {process.ExitCode}";
            };

            while (!string.IsNullOrEmpty((await process.StandardOutput.ReadLineAsync())))
            {
                Output = ((await process.StandardOutput.ReadLineAsync())!);
            }

            process.WaitForExit();
        }


        #endregion

        #region Minecraft root

        private string _root;

        public string Root
        {
            get => _root;
            set => this.RaiseAndSetIfChanged(ref _root, value);
        }
        public async void Browser()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Browser .minecraft directory",
                Directory = @"C:\Users\ahpx\AppData\Roaming"
            };

            Root = await dialog.ShowAsync(GlobalUtility.GetMainWindow());
        }

        #endregion

        #region Java

        private string _java;

        public string Java
        {
            get => _java;
            set => this.RaiseAndSetIfChanged(ref _java, value);
        }
        public async void BrowserJava()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Browser javaw.exe or java.exe",
                Directory = @"C:\Program Files",
                AllowMultiple = false,
                Filters =
                {
                    new ()
                    {
                        Extensions = {"exe"},
                        Name = "executable file"
                    }
                }
            };

            try
            {
                Java = (await dialog.ShowAsync(GlobalUtility.GetMainWindow()))[0];
            }catch { }
            
        }

        #endregion

        #region Launcher name

        private string _launcherName = "ML.RE";

        public string LauncherName
        {
            get => _launcherName;
            set => this.RaiseAndSetIfChanged(ref _launcherName, value);
        }

        #endregion

        #region Memory size

        private string _maxMemory = "2048";

        public string MaxMemory
        {
            get => _maxMemory;
            set => this.RaiseAndSetIfChanged(ref _maxMemory, value);
        }

        private string _minMemory = "1024";

        public string MinMemory
        {
            get => _minMemory;
            set => this.RaiseAndSetIfChanged(ref _minMemory, value);
        }

        #endregion

        #region Player name

        private string _playerName = Guid.NewGuid().ToString("N");

        public string PlayerName
        {
            get => _playerName;
            set => this.RaiseAndSetIfChanged(ref _playerName, value);
        }

        #endregion

        #region Minecraft window

        private string _minecraftWidth;

        public string MinecraftWidth
        {
            get => _minecraftWidth;
            set => this.RaiseAndSetIfChanged(ref _minecraftWidth, value);
        }

        private string _minecraftHeight;

        public string MinecraftHeight
        {
            get => _minecraftHeight;
            set => this.RaiseAndSetIfChanged(ref _minecraftHeight, value);
        }

        private bool _fullscreen = false;

        public bool Fullscreen
        {
            get => _fullscreen;
            set => this.RaiseAndSetIfChanged(ref _fullscreen, value);
        }

        #endregion
    }
}