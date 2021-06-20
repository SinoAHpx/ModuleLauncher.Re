using System;
using System.IO;

namespace ModuleLauncher.Re.Models.Locators
{
    /// <summary>
    /// Description the file structure of a minecraft version
    /// </summary>
    public class LocalVersion
    {
        /// <summary>
        /// .minecraft
        /// </summary>
        public DirectoryInfo Root { get; set; }

        /// <summary>
        /// .minecraft/versions
        /// </summary>
        public DirectoryInfo Versions { get; set; }

        /// <summary>
        /// .minecraft/saves
        /// </summary>
        public DirectoryInfo Saves { get; set; }

        /// <summary>
        /// .minecraft/mods
        /// </summary>
        public DirectoryInfo Mods { get; set; }

        /// <summary>
        /// .minecraft/resourcepacks
        /// </summary>
        public DirectoryInfo ResourcesPacks { get; set; }

        /// <summary>
        /// .minecraft/texturepacks
        /// </summary>
        [Obsolete("Only exist in old version minecraft")]
        public DirectoryInfo TexturePacks { get; set; }

        /// <summary>
        /// .minecraft/libraries
        /// </summary>
        public DirectoryInfo Libraries { get; set; }

        /// <summary>
        /// .minecraft/assets/objects
        /// </summary>
        public DirectoryInfo Assets { get; set; }

        /// <summary>
        /// .minecraft/assets/indexes
        /// </summary>
        public DirectoryInfo AssetsIndexes { get; set; }

        /// <summary>
        /// .minecraft/versions/%ver%/%ver%.jar
        /// </summary>
        public FileInfo Jar { get; set; }

        /// <summary>
        /// .minecraft/versions/%ver%/%ver%.json
        /// </summary>
        public FileInfo Json { get; set; }

        /// <summary>
        /// .minecraft/versions/%ver%
        /// </summary>
        public DirectoryInfo Version { get; set; }

        /// <summary>
        /// .minecraft/versions/%ver%/natives
        /// </summary>
        public DirectoryInfo Natives { get; set; }
    }
}