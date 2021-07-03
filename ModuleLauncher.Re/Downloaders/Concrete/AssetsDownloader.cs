using System.Threading.Tasks;
using ModuleLauncher.Re.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Minecraft;

namespace ModuleLauncher.Re.Downloaders.Concrete
{
    public class AssetsDownloader : DependenciesDownloader
    {
        private readonly AssetsLocator _locator;

        public AssetsDownloader(AssetsLocator locator)
        {
            _locator = locator;
        }
        
        /// <summary>
        /// Download assets via Minecraft object
        /// </summary>
        /// <param name="minecraft"></param>
        /// <param name="ignoreExist"></param>
        public async Task Download(Minecraft minecraft, bool ignoreExist = false)
        {
            var dependencies = await _locator.GetDependencies(minecraft);

            await base.Download(dependencies, ignoreExist);
        }

        /// <summary>
        /// Download assets via minecraft id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ignoreExist"></param>
        public async Task Download(string id, bool ignoreExist = false)
        {
            var dependencies = await _locator.GetDependencies(id);

            await base.Download(dependencies, ignoreExist);
        }

        /// <summary>
        /// Download assets in parallel via minecraft id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ignoreExist"></param>
        /// <param name="maxParallel"></param>
        public async Task DownloadParallel(string id, bool ignoreExist = false, int maxParallel = 5)
        {
            var dependencies = await _locator.GetDependencies(id);

            await base.DownloadParallel(dependencies, ignoreExist, maxParallel);
        }

        /// <summary>
        /// Download assets in parallel via Minecraft object
        /// </summary>
        /// <param name="minecraft"></param>
        /// <param name="ignoreExist"></param>
        /// <param name="maxParallel"></param>
        public async Task DownloadParallel(Minecraft minecraft, bool ignoreExist = false, int maxParallel = 5)
        {
            var dependencies = await _locator.GetDependencies(minecraft);

            await base.DownloadParallel(dependencies, ignoreExist, maxParallel);
        }
    }
}