using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Avalonia.Controls;
using ModuleLauncher.Example.Extensions;
using ModuleLauncher.Re.Locators.Concretes;
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
            var mc = new MinecraftLocator(Root);

            var mcs = await mc.GetLocalMinecrafts();

            mcs.ForEach(x => Minecrafts.Add(x));
        }

        public async void Review()
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
    }
}