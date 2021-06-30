using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModuleLauncher.Re.Locators.Concretes;
using ModuleLauncher.Re.Models.Locators.Dependencies;
using ModuleLauncher.Re.Models.Locators.Minecraft;
using ModuleLauncher.Re.Utils;
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

        public static implicit operator LibrariesLocator(string s)
        {
            return new LibrariesLocator(s);
        }

        /// <summary>
        /// Get library dependencies via minecraft entity
        /// </summary>
        /// <param name="mc"></param>
        /// <param name="excludeNatives">Exclude native dependency or not</param>
        /// <returns></returns>
        public async Task<IEnumerable<Dependency>> GetDependencies(Minecraft mc, bool excludeNatives = false)
        {
            var re = new List<Dependency>();

            var libraries = mc.Raw.Libraries.ToObject<JArray>();

            if (libraries == null)
            {
                throw new JsonException($"{mc.Raw.ToJsonString()} is not a valid minecraft json!");
            }
            
            foreach (var token in libraries)
            {
                if (!IsAddableDependency(token, SystemUtility.GetSystemType())) continue;
                if (!(token is JObject jo)) continue;
                if (IsNativeDependency(token)) continue;

                var rawName = jo.Fetch("name") ??
                              throw new JsonException($"{jo} is a unknown minecraft json format!");

                var relativeUrl = this.GetRelativeUrl(rawName);
                
                var localFile = $"{mc.Locality.Libraries}/{relativeUrl}".BuildPath();

                var dependency = new Dependency
                {
                    Name = relativeUrl.GetFileName(),
                    RelativeUrl = relativeUrl,
                    File = new FileInfo(localFile),
                    Raw = token
                };

                re.Add(dependency);
            }
            
            if (mc.IsInherit())
            {
                try
                {
                    var inheritFrom = await mc.GetInherit();
                    var dependencies = await GetDependencies(inheritFrom, true);
                
                    re.AddRange(dependencies);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"No {mc.Raw.Id} inherited minecraft found!", e);
                }
            }

            return excludeNatives ? re : re.Union(await GetNativeDependencies(mc));
        }

        /// <summary>
        /// Get native librart dependencies via minecraft entity
        /// </summary>
        /// <param name="mc"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<Dependency>> GetNativeDependencies(Minecraft mc)
        {
            var re = new List<Dependency>();

            var libraries = mc.Raw.Libraries.ToObject<JArray>();

            if (libraries == null)
            {
                throw new JsonException($"{mc.Raw.ToJsonString()} is not a valid minecraft json!");
            }
            
            foreach (var token in libraries)
            {
                if (!IsAddableDependency(token, SystemUtility.GetSystemType())) continue;
                if (!(token is JObject jo)) continue;
                if (!IsNativeDependency(token)) continue;

                var relativeUrl = this.AppendNative(jo);
                var localFile = $"{mc.Locality.Libraries}/{relativeUrl}".BuildPath();

                var dependency = new Dependency
                {
                    Name = relativeUrl.GetFileName(),
                    RelativeUrl = relativeUrl,
                    File = new FileInfo(localFile),
                    Raw = token
                };
                
                re.Add(dependency);
            }

            if (!mc.Raw.InheritsFrom.IsNullOrEmpty())
            {
                try
                {
                    var inheritFrom = await _locator.GetLocalMinecraft(mc.Raw.InheritsFrom);
                    var dependencies = await GetNativeDependencies(inheritFrom);
                
                    re.AddRange(dependencies);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"No {mc.Raw.Id} inherited minecraft found!", e);
                }
            }
            
            return re;
        }

        #region Private

        /// <summary>
        /// Determine if incoming json node is a native dependency json node
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        private bool IsNativeDependency(JToken json)
        {
            if (json is JObject jo)
            {
                return jo.ContainsKey("natives");
            }

            throw new JsonException($"{json} is not a JObject!");
        }

        /// <summary>
        /// Determine if incoming json node could be used for set operating system
        /// </summary>
        /// <param name="json"></param>
        /// <param name="system"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        private bool IsAddableDependency(JToken json, DependencySystem system)
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
                            if (rule.IsPathExist("os"))
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
        public async Task<IEnumerable<Dependency>> GetDependencies(string id)
        {
            var mc = await _locator.GetLocalMinecraft(id);

            return await GetDependencies(mc);
        }
        
        /// <summary>
        /// Get native library dependencies via local minecraft id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Dependency>> GetNativeDependencies(string id)
        {
            var mc = await _locator.GetLocalMinecraft(id);

            return await GetNativeDependencies(mc);
        }

        #endregion
    }
}