using System;
using System.Linq;

namespace OdQuestsGenerator.Data
{
	static class IsItOkForCodeGeneration
	{
		public static bool Check(string data)
		{
			return !String.IsNullOrWhiteSpace(data)
				&& (Char.IsLetter(data[0]) || data[0] == '_')
				&& data.All(c => Char.IsLetterOrDigit(c) || c == '_')
			;
		}
	}
}
