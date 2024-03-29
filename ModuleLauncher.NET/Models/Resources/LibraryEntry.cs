﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ModuleLauncher.NET.Models.Resources;

/// <summary>
/// A single library
/// </summary>
public class LibraryEntry
{
    /// <summary>
    /// <example>com/mojang/patchy/1.3.9/patchy-1.3.9.jar</example>
    /// </summary>
    public string RelativeUrl { get; set; }

    /// <summary>
    /// Library entry file info
    /// </summary>
    public FileInfo File { get; set; }

    /// <summary>
    /// Note: native libraries is to extract
    /// </summary>
    public bool IsNative { get; set; }

    /// <summary>
    /// What kind of json file this entry in
    /// </summary>
    public MinecraftType Type { get; set; }

    /// <summary>
    /// Raw library entry json
    /// </summary>
    [JsonIgnore]
    internal JToken Raw { get; set; }
}