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
using System.Collections.Generic;
using System.Text;

namespace Sunburst.Json
{
	internal sealed class JsonParser
	{
		public JsonParser(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				string msg = "Cannot parse a null, zero-length, or white-space-only Json input string.";
				throw new JsonParsingException(msg);
			}

			Input = input;
		}

		private readonly string Input;
		private int Position = 0, LineNumber = 1, ColumnNumber = 1;

		private char GetNextChar()
		{
			if (Position == Input.Length)
			{
				throw new JsonParsingException("Unexpected end of input", LineNumber, ColumnNumber);
			}

			char ch = Input[Position++];
			ColumnNumber++;

			if (ch == '\r')
			{
				if (Input[Position + 1] == '\n') Position++;

				LineNumber++;
				ColumnNumber = 1;

				return '\n';
			}
			else if (ch == '\n')
			{
				LineNumber++;
				ColumnNumber = 1;
			}

			return ch;
		}

		private char PeekNextChar() => Input[Position];

		private void ThrowError(string format, params object[] args)
		{
			string msg = string.Format(format, args);
			throw new JsonParsingException(msg, LineNumber, ColumnNumber);
		}

		private void SkipWhitespace()
		{
			bool continueLoop = true;

			do
			{
				char ch = Input[Position];

				if (ch == '\r')
				{
					// Skip the \n in \r\n, and count it as only one line.
					if (Input[Position + 1] == '\n') Position++;

					LineNumber++;
					ColumnNumber = 1;
				}
				else if (ch == '\n')
				{
					LineNumber++;
					ColumnNumber = 1;
				}
				else if (ch != ' ' && ch != '\t' && ch != '\u0009')
				{
					break;
				}

				Position++;
				ColumnNumber++;
			} while (continueLoop);
		}

		public JsonObject Parse(bool strictMode, JsonType? expectedRootType)
		{
			SkipWhitespace();

			if (expectedRootType.HasValue)
			{
				JsonType expectedValue = expectedRootType.Value;

				if (PeekNextChar() == '[')
				{
					var msg = string.Format("Expected to find a root object of type {0}, but instead " +
											"found an Array.", expectedValue.ToString());
					ThrowError(msg);
				}
				else if (PeekNextChar() == '{')
				{
					var msg = string.Format("Expected to find a root object of type {0}, but instead " +
											"found a Dictionary.", expectedValue.ToString());
					ThrowError(msg);
				}
			}

			if (strictMode)
			{
				if (PeekNextChar() != '[' && PeekNextChar() != '{')
				{
					var msg = string.Format("Strict mode is enabled, and the root object was not of type " +
											"Array or Dictionary. Disable strict mode to parse scalar root types.");
					ThrowError(msg);
				}
			}

			return ParseObject();
		}

		private JsonObject ParseObject()
		{
			SkipWhitespace();

			if (PeekNextChar() == '"') return ParseString();
			else if (NumberChars.Contains(PeekNextChar())) return ParseNumber();
			else if (PeekNextChar() == 't' || PeekNextChar() == 'f') return ParseBoolean();
			else if (PeekNextChar() == 'n') return ParseNull();
			else if (PeekNextChar() == '[') return ParseArray();
			else if (PeekNextChar() == '{') return ParseDictionary();

			ThrowError("Expected scalar or container construct");
			throw new InvalidOperationException("not reached");
		}

		private JsonString ParseString()
		{
			if (GetNextChar() != '"') ThrowError("Expected opening quote in string scalar.");


			var builder = new StringBuilder();
			bool continueLoop = true, previousBackslash = false;

			do
			{
				char ch = GetNextChar();
				while (ch != '"' && ch != '\\')
				{
					builder.Append(ch);
					ch = GetNextChar();
				}

				if (previousBackslash)
				{
					if (ch == 'r') builder.Append('\r');
					else if (ch == 'n') builder.Append('\n');
					else if (ch == 't') builder.Append('\t');
					else if (ch == '"') builder.Append('"');
					else if (ch == '\\') builder.Append('\\');
					else ThrowError("Unrecognized string backslash escape sequence '\\{0}'", ch);

					previousBackslash = false;
				}
				else if (ch == '"')
				{
					continueLoop = false;
				}
				else if (ch == '\\')
				{
					previousBackslash = true;
				}
			} while (continueLoop);

			return new JsonString(builder.ToString());
		}

		private static readonly HashSet<char> NumberChars = new HashSet<char>() {
				'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', 'e', 'E', '+', '-'
			};

		private JsonNumber ParseNumber()
		{
			var builder = new StringBuilder();

			bool continueLoop = true;
			do
			{
				char ch = GetNextChar();

				if (!NumberChars.Contains(ch)) break;
				builder.Append(ch);
			} while (continueLoop);

			decimal value;
			if (!decimal.TryParse(builder.ToString(), out value))
			{
				ThrowError("Invalid numeric literal {0}", builder.ToString());
			}

			return new JsonNumber(value);
		}

		private JsonNull ParseNull()
		{
			if (GetNextChar() != 'n') goto error;
			if (GetNextChar() != 'u') goto error;
			if (GetNextChar() != 'l') goto error;
			if (GetNextChar() != 'l') goto error;

			return JsonNull.Instance;

		error:
			ThrowError("Invalid null literal syntax");
			throw new InvalidOperationException("not reached");
		}

		private JsonBoolean ParseBoolean()
		{
			if (GetNextChar() == 't')
			{
				if (GetNextChar() != 'r') goto error;
				if (GetNextChar() != 'u') goto error;
				if (GetNextChar() != 'e') goto error;

				return JsonBoolean.True;
			}
			else if (GetNextChar() == 'f')
			{
				if (GetNextChar() != 'a') goto error;
				if (GetNextChar() != 'l') goto error;
				if (GetNextChar() != 's') goto error;
				if (GetNextChar() != 'e') goto error;

				return JsonBoolean.False;
			}

		error:
			ThrowError("Invalid boolean literal syntax");
			throw new InvalidOperationException("not reached");
		}

		private JsonArray ParseArray()
		{
			if (GetNextChar() != '[') ThrowError("Expected '[' to begin array construct");
			JsonArray retval = new JsonArray();

			bool continueLoop = true, seenComma = false;
			do
			{
				SkipWhitespace();

				if (PeekNextChar() == ',')
				{
					seenComma = true;
					GetNextChar();
				}
				else if (PeekNextChar() == ']')
				{
					if (seenComma) ThrowError("Comma cannot precede end of array construct");
					else break;

					GetNextChar();
				}
				else
				{
					retval.Add(ParseObject());
					seenComma = false;
				}
			} while (continueLoop);

			if (GetNextChar() != ']') ThrowError("Expected ']' to end array construct");
			return retval;
		}

		private JsonDictionary ParseDictionary()
		{
			if (GetNextChar() != '{') ThrowError("Expected '{{' to begin dictionary construct");
			JsonDictionary retval = new JsonDictionary();

			bool continueLoop = true, seenComma = false;
			do
			{
				SkipWhitespace();

				JsonString key = ParseString();

				SkipWhitespace();
				if (GetNextChar() != ':')
				{
					ThrowError("Expected colon after dictionary key");
					GetNextChar();
				}

				SkipWhitespace();
				JsonObject value = ParseObject();
				SkipWhitespace();

				retval[key.Value] = value;

				if (PeekNextChar() == ',')
				{
					seenComma = true;
					GetNextChar();
				}

				SkipWhitespace();

				if (PeekNextChar() == '}')
				{
					if (seenComma) ThrowError("Comma cannot precede end of dictionary construct");
					continueLoop = false;
				}
				else
				{
					seenComma = false;
				}
			} while (continueLoop);

			if (GetNextChar() != '}') ThrowError("Expected '}' to end dictionary construct");
			return retval;
		}
	}
}
