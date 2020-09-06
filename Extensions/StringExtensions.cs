namespace ModuleLauncher.Re.Extensions
{
    public static class StringExtensions
    {
        public static string ToLibraryPath(this string src, bool isUrl = false)
        {
            var key = isUrl ? '/' : '\\'; 
            
            var split = src.Split(':');
            var s0 = split[0].Replace('.', key);

            return $"{s0}{key}{split[1]}{key}{split[2]}{key}{split[1]}-{split[2]}.jar";
        } 
    }
}