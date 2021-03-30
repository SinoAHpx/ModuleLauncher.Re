using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AHpx.ModuleLauncher.Utils.Extensions
{
    public static class FileExtensions
    {
        public static string GetSha1(this FileInfo ex)
        {
            var fs = new FileStream(ex.FullName, FileMode.Open);
            var sha1 = new SHA1CryptoServiceProvider();
            var result = sha1.ComputeHash(fs);
            
            fs.Close();
            var re = new StringBuilder();
            
            foreach (var b in result)
            {
                re.Append(b.ToString("x2"));
            }

            return re.ToString();
        }
    }
}