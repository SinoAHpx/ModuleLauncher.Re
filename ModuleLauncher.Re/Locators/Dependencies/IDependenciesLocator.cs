using System.Collections.Generic;
using ModuleLauncher.Re.Models.Locators.Dependencies;

namespace ModuleLauncher.Re.Locators.Dependencies
{
    public interface IDependenciesLocator
    {
        public IEnumerable<Dependency> GetDependencies(string id);
    }
}