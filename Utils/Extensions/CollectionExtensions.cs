using System;
using System.Collections.Generic;
using System.Linq;

namespace AHpx.ModuleLauncher.Utils.Extensions
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> ex, Action<T> action)
        {
            ex.ToList().ForEach(action);
        }
    }
}