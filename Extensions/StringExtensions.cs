using System.Drawing;
using System.IO;
using System.Linq;
using Masuit.Tools;
using Masuit.Tools.Media;

namespace ModuleLauncher.Re.Extensions
{
    public static class StringExtensions
    {
        public static string ToLibFormat(this string src, bool isUrl = false)
        {
            var key = isUrl ? '/' : '\\'; 
            
            var split = src.Split(':');
            var s0 = split[0].Replace('.', key);

            return $"{s0}{key}{split[1]}{key}{split[2]}{key}{split[1]}-{split[2]}.jar";
        }
        
        public static string[] ToSrcFormat(this string src)
        {
            var key = src.Contains('/') ? '/' : '\\'; 
            var split = src.Split(key);
            var re = new string[] { };
            var p0 = string.Empty;
            var p1 = string.Empty;
            var p2 = string.Empty;
            
            for (var i = 0; i < split.Length; i++)
            {
                if (i < split.Length - 3)
                    p0 += $"{split[i]}.";
                if (i == split.Length - 2)
                    p2 = split[i];
                if (i == split.Length - 3)
                    p1 = split[i];
            }
            
            return $"{p0.TrimEnd('.')}:{p1}:{p2}".Split(':');
        } 
        
        public static Bitmap Base64ToImage(this string s)
        {
            return s.SaveDataUriAsImageFile();
        }

        public static string GetFileName(this string s)
        {
            return Path.GetFileName(s);
        }
    }
}