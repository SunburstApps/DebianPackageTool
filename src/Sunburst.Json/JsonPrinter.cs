/*
Copyright (c) 2015 Matt Schoen

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System.IO;
using System.Linq;
using System.Text;

namespace Sunburst.Json
{
    internal sealed class JsonPrinter
    {
        private static void ThrowIfNotStrictCompatibleRoot(JsonObject root)
        {
            if (root.Type != JsonType.Dictionary && root.Type != JsonType.Array)
            {
                string msg = "Can only write a root object of type Array or Dictionary in strict mode.";
                throw new JsonWritingException(msg);
            }
        }

        public static void ObjectToStream(JsonObject root, Stream stream, bool compact, bool strict)
        {
            if (!strict) ThrowIfNotStrictCompatibleRoot(root);

            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8, 512, true))
            {
                WriteObject(root, writer, compact, 0);
            }
        }

        public static string ObjectToString(JsonObject root, bool compact, bool strict)
        {
            if (!strict) ThrowIfNotStrictCompatibleRoot(root);

            StringWriter writer = new StringWriter();
            WriteObject(root, writer, compact, 0);
            return writer.ToString();
        }

        private static void WriteIndent(TextWriter writer, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++) writer.Write('\t');
        }

        private static void WriteObject(JsonObject root, TextWriter writer, bool compact, int indentLevel)
        {
            if (root.Type == JsonType.Array)
            {
                WriteArray((JsonArray)root, writer, compact, indentLevel + 1);
            }
            else if (root.Type == JsonType.Dictionary)
            {
                WriteDictionary((JsonDictionary)root, writer, compact, indentLevel + 1);
            }
            else if (root.Type == JsonType.String)
            {
                WriteString(writer, ((JsonString)root).Value);
            }
            else if (root.Type == JsonType.Boolean)
            {
                JsonBoolean value = (JsonBoolean)root;
                writer.Write(value.Value ? "true" : "false");
            }
            else if (root.Type == JsonType.Number)
            {
                JsonNumber value = (JsonNumber)root;
                writer.Write(value.Value.ToString());
            }
            else if (root.Type == JsonType.Null)
            {
                writer.Write("null");
            }
        }

        private static void WriteString(TextWriter writer, string value)
        {
            writer.Write('"');

            foreach (char ch in value)
            {
                if (ch == '\r') writer.Write("\\r");
                else if (ch == '\n') writer.Write("\\n");
                else if (ch == '\t') writer.Write("\\t");
                else if (ch == '"') writer.Write("\\\"");
                else if (ch == '\\') writer.Write("\\\\");
                else writer.Write(ch);
            }

            writer.Write('"');
        }

        private static void WriteDictionary(JsonDictionary dict, TextWriter writer, bool compact, int indentLevel)
        {
            writer.Write('{');
            if (!compact) writer.WriteLine();

            var keys = dict.Keys.ToArray();
            int limit = keys.Length;
            for (int i = 0; i < limit; i++)
            {
                string key = keys[i];
                WriteString(writer, key);
                writer.Write(':');
                if (!compact) writer.Write(' ');

                WriteObject(dict[key], writer, compact, indentLevel + 1);
                if (i != limit) writer.Write(',');
                if (!compact)
                {
                    writer.WriteLine();
                    WriteIndent(writer, indentLevel);
                }
            }

            if (!compact)
            {
                writer.WriteLine();
                WriteIndent(writer, indentLevel);
            }

            writer.Write('}');
        }

        private static void WriteArray(JsonArray root, TextWriter writer, bool compact, int indentLevel)
        {
            writer.Write('[');
            if (!compact) writer.WriteLine();

            int limit = root.Count;
            for (int i = 0; i < limit; i++)
            {
                WriteObject(root[i], writer, compact, indentLevel + 1);
                if (i != limit) writer.Write(',');
                if (!compact) writer.WriteLine();
            }

            if (!compact)
            {
                writer.WriteLine();
                WriteIndent(writer, indentLevel);
            }

            writer.Write(']');
        }
    }
}
