using System.Collections.Generic;
using System.IO;
using Masuit.Tools;
using ModuleLauncher.Re.Extensions;
using ModuleLauncher.Re.Utils;

namespace ModuleLauncher.Re.Minecraft.Locator
{
    public class JavaLocator
    {
        public static IEnumerable<string> GetJavaList()
        {
            const string dir = @"C:\Program Files\Java";
            const string dir32 = @"C:\Program Files (x86)\Java";
            var re = new List<string>();

            if (!Directory.Exists(dir)) return null;
            if (SystemHelper.GetOsBit())
                if (Directory.Exists(dir32))
                    Directory.GetDirectories(dir32).ForEach(x => { re.Add(x.GetJavaPath()); });

            Directory.GetDirectories(dir).ForEach(x => { re.Add(x.GetJavaPath()); });
            return re;
        }
    }
}