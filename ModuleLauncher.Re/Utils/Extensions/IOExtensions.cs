using System.IO;
using System.Threading.Tasks;

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
        /// e.g. assets/indexes -> assets/indexes/1.8.json
        /// </summary>
        /// <param name="info"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public static FileInfo GetSubFileInfo(this DirectoryInfo info, string sub)
        {
            var dir = info.FullName;
            if (!dir.EndsWith("\\"))
            {
                dir += "\\";
            }

            return new FileInfo($@"{dir}{sub.TrimStart('\\')}");
        }

        /// <summary>
        /// Equals to File.WriteAllText
        /// </summary>
        /// <param name="info"></param>
        /// <param name="content"></param>
        public static async Task WriteAllText(this FileInfo info, string content)
        {
            await File.WriteAllTextAsync(info.FullName, content);
        }

        /// <summary>
        /// Equals to File.ReadAllText
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static async Task<string> ReadAllText(this FileInfo info)
        {
            return await File.ReadAllTextAsync(info.FullName);
        }
    }
}