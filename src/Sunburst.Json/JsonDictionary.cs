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
	public sealed class JsonDictionary : JsonObject, IDictionary<string, JsonObject>,
	IReadOnlyDictionary<string, JsonObject>, IEquatable<JsonDictionary>
	{
		private Dictionary<string, JsonObject> body = new Dictionary<string, JsonObject>();

		public override JsonType Type => JsonType.Dictionary;
		public override bool IsContainer => true;

		public JsonObject this[string key]
		{
			get
			{
				return body[key];
			}

			set
			{
				body[key] = value;
			}
		}

		public int Count => body.Count;
		public bool IsReadOnly => false;
		public ICollection<string> Keys => body.Keys;
		IEnumerable<string> IReadOnlyDictionary<string, JsonObject>.Keys => body.Keys;
		public ICollection<JsonObject> Values => body.Values;
		IEnumerable<JsonObject> IReadOnlyDictionary<string, JsonObject>.Values => body.Values;

		public void Add(KeyValuePair<string, JsonObject> item) => body.Add(item.Key, item.Value);
		public void Add(string key, JsonObject value) => body.Add(key, value);
		public void Clear() => body.Clear();
		public bool ContainsKey(string key) => body.ContainsKey(key);
		public bool Equals(JsonDictionary other) => body.Equals(other.body);
		public IEnumerator<KeyValuePair<string, JsonObject>> GetEnumerator() => body.GetEnumerator();
		public bool Remove(KeyValuePair<string, JsonObject> item) => body.Remove(item.Key);
		public bool Remove(string key) => body.Remove(key);
		public bool TryGetValue(string key, out JsonObject value) => body.TryGetValue(key, out value);
		IEnumerator IEnumerable.GetEnumerator() => body.GetEnumerator();

		public bool Contains(KeyValuePair<string, JsonObject> item)
		{
			return body.ContainsKey(item.Key) && body.ContainsValue(item.Value);
		}

		public void CopyTo(KeyValuePair<string, JsonObject>[] array, int arrayIndex)
		{
			List<KeyValuePair<string, JsonObject>> pairs = new List<KeyValuePair<string, JsonObject>>();

			foreach (KeyValuePair<string, JsonObject> pair in body)
			{
				pairs.Add(new KeyValuePair<string, JsonObject>(pair.Key, pair.Value));
			}

			pairs.CopyTo(array, arrayIndex);
		}

		public override bool Equals(JsonObject other)
		{
			if (other.Type != JsonType.Dictionary) return false;

			JsonDictionary otherDictionary = (JsonDictionary)other;
			if (otherDictionary == null) return false;

			return Equals(otherDictionary);
		}

		public override bool Equals(object obj)
		{
			if (!GetType().Equals(obj.GetType())) return false;
			return Equals((JsonDictionary)obj);
		}

		public override int GetHashCode()
		{
#pragma warning disable RECS0025 // Non-readonly field referenced in 'GetHashCode()'
			return body.GetHashCode();
#pragma warning restore RECS0025 // Non-readonly field referenced in 'GetHashCode()'
		}

		public override string ToString()
		{
			return string.Format("[JsonDictionary: Count={0}]", Count);
		}
	}
}
