using System.Collections.Generic;
using System.IO;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Minecraft;
using ModuleLauncher.Re.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.Re.Locators.Dependencies
{
    public class LibrariesLocator : IDependenciesLocator
    {
        private readonly MinecraftLocator _locator;

        public LibrariesLocator(MinecraftLocator locator)
        {
            _locator = locator;
        }

        /// <summary>
        /// Get libraries dependencies via minecraft entity
        /// </summary>
        /// <param name="minecraft"></param>
        /// <returns></returns>
        public IEnumerable<Dependency> GetDependencies(Minecraft mc)
        {
            var re = new List<Dependency>();

            var libraries = mc.Raw.Libraries.ToObject<JArray>();

            if (libraries == null)
            {
                throw new JsonException($"{mc.Raw.ToJsonString()} is not a valid minecraft json!");
            }
            
            foreach (var token in libraries)
            {
                if (token is JObject jo)
                {
                    var rawName = jo.Fetch("name") ??
                                  throw new JsonException($"{token} is a unknown minecraft json format!");

                    var relativeUrl = this.GetRelativeUrl(rawName);
                    var localFile = $"{mc.Locality.Libraries}\\{relativeUrl.Replace()}";

                    var dependence = new Dependency
                    {
                        Name = relativeUrl.GetFileName(),
                        RelativeUrl = relativeUrl,
                        File = new FileInfo(localFile)
                    };
                    
                    re.Add(dependence);
                }
            }

            return re;
        }
        
        /// <summary>
        /// Get libraries dependencies via local minecraft id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public IEnumerable<Dependency> GetDependencies(string id)
        {
            var mc = _locator.GetLocalMinecraft(id);

            return GetDependencies(mc);
        }
        
        public IEnumerable<Dependency> GetNativeDependencies(string id)
        {
            
        }

        private bool IsNativeDependence()
        {
            
        }
    }
}