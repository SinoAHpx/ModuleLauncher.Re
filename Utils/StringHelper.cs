using System;
using System.Collections.Generic;
using System.Linq;

namespace ModuleLauncher.Re.Utils
{
    public class StringHelper
    {
        public static IEnumerable<string> GetAlphabets()
        {
            var ls = new List<string>();
            for (var i = 65; i <= 90; i++) ls.Add(((char) i).ToString());
            var re = new List<string>();
            ls.ForEach(x =>
            {
                re.Add(x.ToLower());
                re.Add(x);
            });

            return re;
        }

        public static string GetRandomString(string s, int length = 10)
        {
            var ls = GetAlphabets().ToList();
            var re = "";
            ls.AddRange(new[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"});

            var random = new Random();
            for (var i = 0; i < length; i++)
                re += ls[random.Next(ls.Count)];

            return re;
        }
    }
}