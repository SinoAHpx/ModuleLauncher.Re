using System.ComponentModel;
using System.Runtime.Serialization;

namespace ModuleLauncher.Re.Models.Locators.Dependencies
{
    public enum DependencySystem
    {
        [Description("windows")]
        Windows,
        [Description("linux")]
        Linux,
        [Description("osx")]
        Mac
    }
}