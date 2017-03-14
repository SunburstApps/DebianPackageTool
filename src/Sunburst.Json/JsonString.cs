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

namespace Sunburst.Json
{
    public sealed class JsonString : JsonObject, IEquatable<JsonString>, IEquatable<string>
    {
        public JsonString(string val)
        {
            Value = val;
        }

        public override JsonType Type => JsonType.String;
        public string Value { get; private set; }

        public bool Equals(JsonString other) => Value == other.Value;
        public bool Equals(string other) => Value == other;

        public override bool Equals(JsonObject other)
        {
            if (other.Type != JsonType.String) return false;

            JsonString otherString = other as JsonString;
            if (otherString == null) return false;

            return otherString.Value == Value;
        }

        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(GetType())) return false;
            return Equals((JsonString)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[JsonString: {0}]", Value);
        }
    }
}
