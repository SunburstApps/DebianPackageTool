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
using System.Collections;
using System.Collections.Generic;

namespace Sunburst.Json
{
	public sealed class JsonArray : JsonObject, IList<JsonObject>, IReadOnlyList<JsonObject>, IEquatable<JsonArray>
	{
		private List<JsonObject> body = new List<JsonObject>();

		public override JsonType Type => JsonType.Array;
		public override bool IsContainer => true;

		public JsonObject this[int index]
		{
			get
			{
				return body[index];
			}

			set
			{
				body[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return body.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Add(JsonObject item)
		{
			body.Add(item);
		}

		public void Clear()
		{
			body.Clear();
		}

		public bool Contains(JsonObject item)
		{
			return body.Contains(item);
		}

		public void CopyTo(JsonObject[] array, int arrayIndex)
		{
			body.CopyTo(array, arrayIndex);
		}

		public override bool Equals(JsonObject other)
		{
			if (other.Type != JsonType.Array) return false;

			JsonArray otherArray = other as JsonArray;
			if (otherArray == null) return false;

			return Equals(otherArray);
		}

		public bool Equals(JsonArray other)
		{
			return body.Equals(other.body);
		}

		public IEnumerator<JsonObject> GetEnumerator()
		{
			return body.GetEnumerator();
		}

		public int IndexOf(JsonObject item)
		{
			return body.IndexOf(item);
		}

		public void Insert(int index, JsonObject item)
		{
			body.Insert(index, item);
		}

		public bool Remove(JsonObject item)
		{
			return body.Remove(item);
		}

		public void RemoveAt(int index)
		{
			body.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return body.GetEnumerator();
		}

		public override bool Equals(object obj)
		{
			if (!obj.GetType().Equals(GetType())) return false;
			return Equals((JsonArray)obj);
		}

		public override int GetHashCode()
		{
#pragma warning disable RECS0025 // Non-readonly field referenced in 'GetHashCode()'
			return body.GetHashCode();
#pragma warning restore RECS0025 // Non-readonly field referenced in 'GetHashCode()'
		}

		public override string ToString()
		{
			return string.Format("[JsonArray: Count={0}]", Count);
		}
	}
}
