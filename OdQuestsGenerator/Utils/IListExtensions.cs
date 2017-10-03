using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OdQuestsGenerator.Utils
{
	public static class IListExtensions
	{
		public static void Move(this IList list, int from, int to)
		{
			var objToMove = list[from];
			list.RemoveAt(from);
			list.Insert(to, objToMove);
		}

		public static void AddIfNotContains<TType>(this IList<TType> list, TType item)
		{
			if (!list.Contains(item)) {
				list.Add(item);
			}
		}

		public static TType FirstOfTypeOrDefault<TType>(this IList list)
		{
			foreach (var i in list) {
				if (i is TType) {
					return (TType)i;
				}
			}

			return default(TType);
		}

		public static TType FirstOfType<TType>(this IList list)
		{
			foreach (var i in list) {
				if (i is TType) {
					return (TType)i;
				}
			}

			throw new System.Exception("Couldn't find element in list with type");
		}

		public static bool Exists<TType>(this IList list)
		{
			foreach (var i in list) {
				if (i is TType) {
					return true;
				}
			}

			return false;
		}
	}
}
