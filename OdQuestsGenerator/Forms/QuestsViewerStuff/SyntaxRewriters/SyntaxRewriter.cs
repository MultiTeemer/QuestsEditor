using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FindSymbols;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.SyntaxRewriters
{
	abstract class SyntaxRewriter : CSharpSyntaxRewriter
	{
		protected readonly Solution Solution;
		protected readonly Compilation Compilation;

		public abstract IReadOnlyList<DocumentId> GetDocumentsIdsToModify();

		protected SyntaxRewriter(Solution solution, Compilation compilation)
		{
			Solution = solution;
			Compilation = compilation;
		}

		protected IReadOnlyList<DocumentId> GetIdsOfDocsWithReferencesToSymbol(ISymbol symbol)
		{
			return SymbolFinder
				.FindReferencesAsync(symbol, Solution)
				.Result
				.SelectMany(r => r.Locations.Select(l => l.Document.Id))
				.ToList();
		}
	}
}
