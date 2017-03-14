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

using System;
using System.IO;

namespace Sunburst.Json
{
	public abstract class JsonObject : IEquatable<JsonObject>
	{
		public abstract JsonType Type { get; }
		public abstract bool Equals(JsonObject other);
		public virtual bool IsContainer => false;

		public static JsonObject Parse(string input)
		{
			return Parse(input, false);
		}

		public static JsonDictionary ParseDictionary(string input)
		{
			JsonObject output = Parse(input, true, JsonType.Dictionary);
			if (output.Type != JsonType.Dictionary)
			{
				string msg = string.Format("{0}() expects to parse a dictionary root type, but instead received {1}",
										  nameof(ParseDictionary), output.Type.ToString());
				throw new JsonParsingException(msg, 1, 1);
			}

			return (JsonDictionary)output;
		}

		public static JsonArray ParseArray(string input)
		{
			JsonObject output = Parse(input, true, JsonType.Dictionary);
			if (output.Type != JsonType.Array)
			{
				string msg = string.Format("{0}() expects to parse an array root type, but instead received {1}",
										   nameof(ParseArray), output.Type.ToString());
				throw new JsonParsingException(msg, 1, 1);
			}

			return (JsonArray)output;
		}

		private static JsonObject Parse(string input, bool strictMode, JsonType? expectedRootType = null)
		{
			JsonParser parser = new JsonParser(input);
			return parser.Parse(strictMode, expectedRootType);
		}

		public static string WriteToString(JsonObject root, JsonWritingFlags flags)
		{
			return JsonPrinter.ObjectToString(root, flags.HasFlag(JsonWritingFlags.Compact),
											  flags.HasFlag(JsonWritingFlags.Strict));
		}

		public static void WriteToStream(Stream stream, JsonObject root, JsonWritingFlags flags)
		{
			JsonPrinter.ObjectToStream(root, stream,
									   flags.HasFlag(JsonWritingFlags.Compact),
									   flags.HasFlag(JsonWritingFlags.Strict));
		}
	}

	[Flags]
	public enum JsonWritingFlags : uint
	{
		/// <summary>
		/// The Json output should not contain any extra whitespace.
		/// </summary>
		Compact = 1,

		/// <summary>
		/// The Json writer should throw an exception if the root object
		/// is not an array or a dictionary.
		/// </summary>
		Strict = 2
	}

	public enum JsonType
	{
		Null,
		Boolean,
		String,
		Number,
		Dictionary,
		Array
	}

	public class JsonParsingException : Exception
	{
		public JsonParsingException() : this("Invalid Json document.") { }
		public JsonParsingException(string msg) : this(msg, null) { }
		public JsonParsingException(string msg, Exception inner) : base(msg, inner) { }

		public JsonParsingException(string msg, int line, int column) : this(msg)
		{
			LineNumber = line;
			ColumnNumber = column;
		}

		public JsonParsingException(string msg, Exception inner, int line, int column) : this(msg, inner)
		{
			LineNumber = line;
			ColumnNumber = column;
		}

		public int LineNumber { get; set; } = -1;
		public int ColumnNumber { get; set; } = -1;
	}

	public class JsonWritingException : Exception
	{
		public JsonWritingException() : this("Error occurred while writing Json document.") { }
		public JsonWritingException(string msg) : this(msg, null) { }
		public JsonWritingException(string msg, Exception inner) : base(msg, inner) { }
	}
}
