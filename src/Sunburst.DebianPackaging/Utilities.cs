using System;
using System.Collections.Generic;
using System.IO;

namespace Sunburst.DebianPackaging
{
    internal static class Utilities
    {
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue default_value)
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

        public static FileInfo GetFile(this DirectoryInfo dir, string name)
        {
            return new FileInfo(Path.Combine(dir.FullName, name));
        }

        public static DirectoryInfo GetSubdirectory(this DirectoryInfo dir, string name)
        {
            return new DirectoryInfo(Path.Combine(dir.FullName, name));
        }

        public static T ParseEnum<T>(string text, bool ignoreCase = true)
        {
            Type type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException($"Cannot call {nameof(ParseEnum)} with non-enum type {type.FullName}");

            return (T)Enum.Parse(type, text, ignoreCase);
        }
    }
}
