using ModuleLauncher.Re.Models.Locators.Dependencies;
using ReactiveUI;

namespace ModuleLauncher.Example.ViewModels.Downloaders
{
    public class DependencyDownloaderItemViewModel : ViewModelBase
    {
        public Dependency Dependency { get; set; }

        private double _progress;

        public double Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }
    }
}