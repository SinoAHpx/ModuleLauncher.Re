using System.Collections.Generic;
using System.Threading.Tasks;
using ModuleLauncher.Re.Models.Locators.Dependencies;

namespace ModuleLauncher.Re.Locators.Dependencies
{
    public interface IDependenciesLocator
    {
        public Task<IEnumerable<Dependency>> GetDependencies(string id);
    }
}