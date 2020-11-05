using System;
using System.Collections.Generic;
using System.Linq;

namespace AHpx.ModuleLauncher.Utils.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            var hash = new HashSet<TKey>();
            return source.Where(p => hash.Add(keySelector(p)));
        }
    }
}