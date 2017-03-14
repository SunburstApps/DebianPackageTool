using System.Collections.Generic;

namespace Sunburst.DebianPackaging
{
    internal static class Utilities
    {
        public static TValue TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key, TValue default_value)
        {
            bool success = dict.TryGetValue(key, out TValue value);
            return success ? value : default_value;
        }

        public static string Format(this string format, params (string name, string value)[] arguments)
        {
            string result = format;
            result = result.Replace("{{", "{\x01");
            result = result.Replace("}}", "\x01}");

            foreach (var pair in arguments)
            {
                string token = string.Concat("{", pair.name, "}");
                result = result.Replace(token, pair.value);
            }

            result = result.Replace("\x01", "");
            return result;
        }
    }
}
