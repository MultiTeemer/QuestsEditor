using System.Collections;
using System.Collections.Generic;

namespace OdQuestsGenerator.Utils
{
	class TwoWayDictionary<TType1, TType2> : IEnumerable<KeyValuePair<TType1, TType2>>
	{
		private readonly Dictionary<TType1, TType2> dict1 = new Dictionary<TType1, TType2>();
		private readonly Dictionary<TType2, TType1> dict2 = new Dictionary<TType2, TType1>();

		public IReadOnlyCollection<TType1> Values1 => dict1.Keys;
		public IReadOnlyCollection<TType2> Values2 => dict2.Keys;

		public TType2 this[TType1 idx]
		{
			get => dict1[idx];
			set
			{
				dict1[idx] = value;
				dict2[value] = idx;
			}
		}

		public TType1 this[TType2 idx]
		{
			get => dict2[idx];
			set
			{
				dict1[value] = idx;
				dict2[idx] = value;
			}
		}

		public void Add(TType1 val1, TType2 val2)
		{
			dict1.Add(val1, val2);
			dict2.Add(val2, val1);
		}

		public void Remove(TType1 val)
		{
			if (dict1.ContainsKey(val)) {
				var key = dict1[val];
				dict2.Remove(key);

				dict1.Remove(val);
			}
		}

		public void Remove(TType2 val)
		{
			if (dict2.ContainsKey(val)) {
				var key = dict2[val];
				dict1.Remove(key);

				dict2.Remove(val);
			}
		}

		public void Clear()
		{
			dict1.Clear();
			dict2.Clear();
		}

		public bool Contains(TType1 val) => dict1.ContainsKey(val);
		public bool Contains(TType2 val) => dict2.ContainsKey(val);

		public IEnumerator<KeyValuePair<TType1, TType2>> GetEnumerator() => dict1.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => dict1.GetEnumerator();
	}
}
