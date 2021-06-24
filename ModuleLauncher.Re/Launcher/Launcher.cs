using System.Diagnostics;
using System.Threading.Tasks;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Authenticators;

namespace ModuleLauncher.Re.Launcher
{
    public class Launcher
    {
        private readonly MinecraftLocator _minecraftLocator;
        
        public string Java { get; set; }

        public AuthenticateResult Authentication { get; set; }

        public Launcher(MinecraftLocator minecraftLocator)
        {
            _minecraftLocator = minecraftLocator;
        }

        public async Task<Process> Launch(string id)
        {
            var mc = await _minecraftLocator.GetLocalMinecraft(id);

            return null;
        }
    }
}