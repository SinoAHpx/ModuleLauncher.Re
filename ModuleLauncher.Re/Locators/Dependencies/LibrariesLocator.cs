using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// <param name="mc"></param>
        /// <param name="excludeNatives">Exclude native dependency or not</param>
        /// <returns></returns>
        public IEnumerable<Dependency> GetDependencies(Minecraft mc, bool excludeNatives = true)
        {
            var re = new List<Dependency>();

            if (!mc.Raw.InheritsFrom.IsNullOrEmpty())
            {
                re.AddRange(GetDependencies(_locator.GetLocalMinecraft(mc.Raw.InheritsFrom), excludeNatives));
            }

            var libraries = mc.Raw.Libraries.ToObject<JArray>();

            if (libraries == null)
            {
                throw new JsonException($"{mc.Raw.ToJsonString()} is not a valid minecraft json!");
            }
            
            foreach (var token in libraries)
            {
                if (!IsAddableDependency(token)) continue;
                if (!(token is JObject jo)) continue;
                if (excludeNatives && IsNativeDependency(token)) continue;

                re.Add(BuildDependency(jo, mc));
            }

            return re;
        }

        public IEnumerable<Dependency> GetNativeDependencies(Minecraft mc)
        {
            var re = new List<Dependency>();
            
            if (!mc.Raw.InheritsFrom.IsNullOrEmpty())
            {
                re.AddRange(GetNativeDependencies(_locator.GetLocalMinecraft(mc.Raw.InheritsFrom)));
            }

            var libraries = mc.Raw.Libraries.ToObject<JArray>();

            if (libraries == null)
            {
                throw new JsonException($"{mc.Raw.ToJsonString()} is not a valid minecraft json!");
            }
            
            foreach (var token in libraries)
            {
                if (!IsAddableDependency(token)) continue;
                if (!(token is JObject jo)) continue;
                if (!IsNativeDependency(token)) continue;
                
                re.Add(BuildDependency(jo, mc));
            }

            return re;
        }

        #region Private

        private Dependency BuildDependency(JToken jo, Minecraft mc)
        {
            var rawName = jo.Fetch("name") ??
                          throw new JsonException($"{jo} is a unknown minecraft json format!");

            var relativeUrl = this.GetRelativeUrl(rawName);
            var localFile = $"{mc.Locality.Libraries}\\{relativeUrl.Replace()}";

            var dependency = new Dependency
            {
                Name = relativeUrl.GetFileName(),
                RelativeUrl = relativeUrl,
                File = new FileInfo(localFile)
            };

            return dependency;
        }
        
        private bool IsNativeDependency(JToken json)
        {
            if (json is JObject jo)
            {
                return jo.ContainsKey("natives");
            }

            throw new JsonException($"{json} is not a JObject!");
        }

        private bool IsAddableDependency(JToken json, DependencySystem system = DependencySystem.Linux)
        {
            if (json is JObject jo)
            {
                if (jo.ContainsKey("rules"))
                {
                    var rules = jo.Fetch("rules").ToJArray();
                    var os = system.GetDependencySystemString();

                    var disallows = rules.Where(x => x.Fetch("action") == "disallow").ToList();
                    if (disallows.Any())
                    {
                        foreach (var token in disallows)
                        {
                            if (token.Fetch("os.name") == os)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        foreach (var rule in rules)
                        {
                            if (rule.ContainsKey("os"))
                            {
                                return rule.Fetch("os.name") == os && rule.Fetch("action") == "allow";
                            }
                        
                            return rule.Fetch("action") == "allow";
                        }
                    }
                }

                return true;
            }

            throw new JsonException($"{json} is not a JObject!");
        }

        #endregion
        
        #region Overloads

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
            var mc = _locator.GetLocalMinecraft(id);

            return GetNativeDependencies(mc);
        }

        #endregion
    }
}