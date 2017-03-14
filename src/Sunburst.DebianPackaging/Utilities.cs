using System.Collections.Generic;

namespace Sunburst.DebianPackaging
{
    internal static class Utilities
    {
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key, TValue default_value)
        {
            bool success = dict.TryGetValue(key, out TValue value);
            return success ? value : default_value;
        }
    }
}
