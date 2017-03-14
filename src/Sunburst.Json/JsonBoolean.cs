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
    public sealed class JsonBoolean : JsonObject, IEquatable<JsonBoolean>, IEquatable<bool>
    {
        public static readonly JsonBoolean True = new JsonBoolean(true);
        public static readonly JsonBoolean False = new JsonBoolean(false);

        public JsonBoolean(bool val)
        {
            Value = val;
        }

        public override JsonType Type => JsonType.Boolean;
        public bool Value { get; private set; }

        public bool Equals(JsonBoolean other) => Value == other.Value;
        public bool Equals(bool other) => Value == other;

        public override bool Equals(JsonObject other)
        {
            if (other.Type != JsonType.Boolean) return false;

            JsonBoolean otherBoolean = other as JsonBoolean;
            if (otherBoolean == null) return false;

            return otherBoolean.Value == Value;
        }

        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(GetType())) return false;
            return Equals((JsonBoolean)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString()
        {
            return string.Format("[JsonBoolean: {0}]", Value);
        }
    }
}
