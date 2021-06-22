using System.Collections.Generic;
using ModuleLauncher.Re.Models.Locators.Dependencies;

namespace ModuleLauncher.Re.Locators.Dependencies
{
    public interface IDependenciesLocator
    {
        public IEnumerable<Dependence> GetDependencies(string id);
    }
}