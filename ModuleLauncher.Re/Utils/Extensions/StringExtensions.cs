namespace ModuleLauncher.Re.Utils.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// A string extension method that equals to string.IsNullOrEmpty()
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
    }
}