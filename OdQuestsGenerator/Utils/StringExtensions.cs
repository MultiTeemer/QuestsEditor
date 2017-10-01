using System.Text.RegularExpressions;

namespace OdQuestsGenerator.Utils
{
	public static class StringExtensions
	{
		public static string Erase(this string str, string pattern)
		{
			return Regex.Replace(str, pattern, "");
		}
	}
}
