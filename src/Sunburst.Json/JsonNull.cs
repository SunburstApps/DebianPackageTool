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
    public sealed class JsonNull : JsonObject, IEquatable<JsonNull>
    {
        public static readonly JsonNull Instance = new JsonNull();

        private JsonNull() { }

        public override JsonType Type => JsonType.Null;

        public override bool Equals(JsonObject other)
        {
            if (other.Type != JsonType.Null) return false;
            JsonNull otherNull = other as JsonNull;
            if (otherNull == null) return false;

            return Equals(otherNull);
        }

        // All JsonNull instances are equal.
        public bool Equals(JsonNull other) => true;

        public override bool Equals(object obj)
        {
            if (!obj.GetType().Equals(GetType())) return false;
            return Equals((JsonNull)obj);
        }

        public override int GetHashCode()
        {
            // This is here just to suppress a compiler warning.
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "[JsonNull]";
        }
    }
}
