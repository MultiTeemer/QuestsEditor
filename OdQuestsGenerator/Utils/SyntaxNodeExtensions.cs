using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace OdQuestsGenerator.Utils
{
	public static class SyntaxNodeExtensions
	{
		public static TNodeType GetFirstOfType<TNodeType>(this SyntaxNode node)
			where TNodeType : SyntaxNode
		{
			return node.DescendantNodes().FirstOrDefault(n => n is TNodeType) as TNodeType;
		}

		public static IEnumerable<TNodeType> OfType<TNodeType>(this SyntaxNode node)
			where TNodeType : SyntaxNode
		{
			return node.DescendantNodes().OfType<TNodeType>().Select(n => n as TNodeType);
		}
	}
}
