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
    public sealed class JsonNumber : JsonObject, IEquatable<JsonNumber>, IEquatable<decimal>
    {
        public JsonNumber(decimal val)
        {
            Value = val;
        }

        public override JsonType Type => JsonType.Number;
        public decimal Value { get; private set; }
        public long IntegralValue => Convert.ToInt64(Math.Round(Value));

        public bool Equals(JsonNumber other) => Value == other.Value;
        public bool Equals(decimal other) => Value == other;

        public override bool Equals(JsonObject other)
        {
            if (other.Type != JsonType.Number) return false;

            JsonNumber otherNumber = other as JsonNumber;
            if (otherNumber == null) return false;

            return Value.Equals(otherNumber.Value);
        }

        public override bool Equals(object obj)
        {
            if (!GetType().Equals(obj.GetType())) return false;
            return Equals((JsonNumber)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[JsonNumber: Value={0}]", Value);
        }
    }
}
