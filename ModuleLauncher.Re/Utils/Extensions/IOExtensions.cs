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

        /// <summary>
        /// E.g. .minecraft => .minecraft/versions
        /// </summary>
        /// <param name="info"></param>
        /// <param name="sub">e.g. versions (without any slash)</param>
        /// <returns></returns>
        public static DirectoryInfo ToSubDirectoryInfo(this DirectoryInfo info, string sub)
        {
            var dir = info.FullName;
            if (!dir.EndsWith("\\"))
            {
                dir += "\\";
            }

            return new DirectoryInfo($@"{dir}{sub.TrimStart('\\')}");
        }

        /// <summary>
        /// Equals to File.ReadAllText
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string ReadAllText(this FileInfo info)
        {
            return File.ReadAllText(info.FullName);
        }
    }
}