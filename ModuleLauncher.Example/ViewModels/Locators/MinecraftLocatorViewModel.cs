using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Minecraft;
using MoreLinq;
using ReactiveUI;

namespace ModuleLauncher.Example.ViewModels.Locators
{
    public class MinecraftLocatorViewModel : ViewModelBase
    {
        private string _root;

        public string Root
        {
            get => _root;
            set => this.RaiseAndSetIfChanged(ref _root, value);
        }

        public ObservableCollection<Minecraft> Minecrafts { get; set; } = new();

        private Minecraft _selectedMc;

        public Minecraft SelectedMc
        {
            get => _selectedMc;
            set => this.RaiseAndSetIfChanged(ref _selectedMc, value);
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

        public async void Review()
        {
            try
            {
                var locality = "";

                foreach (var info in SelectedMc.Locality.GetType().GetProperties())
                {
                    locality += $"{info.Name}: {info.GetValue(SelectedMc.Locality)}\r\n";
                }

                var args = SelectedMc.Raw.Arguments?.ToString()?.Replace(Environment.NewLine, "");
                await MessageBoxEx.Show($"Id: {SelectedMc.Raw.Id}" +
                                        $"\r\nLibraries count: {SelectedMc.Raw.Libraries.Count}" +
                                        $"\r\nArguments: {(string.IsNullOrEmpty(args) ? SelectedMc.Raw.MinecraftArguments : args)}" +
                                        $"\r\nInherit from: {SelectedMc.Raw.InheritsFrom}" +
                                        $"\r\n{locality}");
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show(e.Message);
            }
            
        }

        public ObservableCollection<Dependency> Libraries { get; set; } = new();

        public async void GetLibraries()
        {
            try
            {
                var librariesLocator = new LibrariesLocator(Root);

                var dependencies = await librariesLocator.GetDependencies(SelectedMc?.Raw.Id);

                Libraries.Clear();
                dependencies.ForEach(x => Libraries.Add(x));
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show($"{e.Message}\r\nTry to get and select a minecraft first");
            }
            
        }

        public ObservableCollection<Dependency> Assets { get; set; } = new();

        public async void GetAssets()
        {
            try
            {
                var assetsLocator = new AssetsLocator(Root);

                var dependencies = await assetsLocator.GetDependencies(SelectedMc?.Raw.Id);

                Assets.Clear();
                dependencies.ForEach(x => Assets.Add(x));
            }
            catch (Exception e)
            {
                await MessageBoxEx.Show($"{e.Message}\r\nTry to get and select a minecraft first");
            }
        }

    }
}