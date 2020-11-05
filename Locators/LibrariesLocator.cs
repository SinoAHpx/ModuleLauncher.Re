﻿using System.Collections.Generic;
using System.IO;
using AHpx.ModuleLauncher.Data.Locators;
using AHpx.ModuleLauncher.Utils.Extensions;

namespace AHpx.ModuleLauncher.Locators
{
    public class LibrariesLocator : MinecraftLocator
    {
        public LibrariesLocator(string location = null) : base(location){}

        public IEnumerable<Library> GetLibraries(string version)
        {
            var re = new List<Library>();
            var mc = GetMinecraft(version);
            var json = mc.Json;
            
            foreach (var token in json.Libraries)
            {
                re.Add(new Library
                {
                    File = new FileInfo($@"{mc.File.Libraries}\{token["name"]?.ToString().ToLibraryFile()!}"),
                    Name = token["name"]?.ToString(),
                    Url = "" //TODO: SHOULD BE SUPPORT FOR MULTI DOWNLOAD MIRROR
                });
            }

            return re;
        }
    }
}