using System.Collections;

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
	}
}
