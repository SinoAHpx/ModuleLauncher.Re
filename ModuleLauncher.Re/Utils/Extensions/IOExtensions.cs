using System.IO;

namespace ModuleLauncher.Re.Utils.Extensions
{
    public static class IOExtensions
    {
        /// <summary>
        /// Convert a directory path string to DirectoryInfo
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DirectoryInfo ToDirectoryInfo(this string s)
        {
            return new DirectoryInfo(s);
        }

        /// <summary>
        /// Convert a file path string to FileInfo
        /// </summary>
        /// <returns></returns>
        public static FileInfo ToFileInfo(this string s)
        {
            return new FileInfo(s);
        }
    }
}