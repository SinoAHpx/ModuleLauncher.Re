using System.IO;

namespace ModuleLauncher.Re.Utils
{
    public class SystemHelper
    {
        /// <summary>
        ///     祖传系统位数判断方法
        /// </summary>
        /// <returns>x64 => true x86 => false</returns>
        public static bool GetOsBit()
        {
            return Directory.Exists(@"C:\Program Files (x86)");
        }

        public static string GetOsBitStr()
        {
            return GetOsBit() ? "64" : "32";
        }
    }
}