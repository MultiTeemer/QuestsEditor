using System.Linq;
using Microsoft.CodeAnalysis;

namespace OdQuestsGenerator.Utils
{
	public static class SyntaxNodeExtensions
	{
		public static TNodeType GetFirstOfType<TNodeType>(this SyntaxNode node)
			where TNodeType : SyntaxNode
		{
			return node.ChildNodes().FirstOrDefault(n => n is TNodeType) as TNodeType;
		}
	}
}
