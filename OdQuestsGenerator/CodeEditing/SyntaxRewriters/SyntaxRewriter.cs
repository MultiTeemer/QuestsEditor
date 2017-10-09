using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FindSymbols;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.CodeEditing.SyntaxRewriters
{
	abstract class SyntaxRewriter : CSharpSyntaxRewriter
	{
		protected readonly Solution Solution;
		protected readonly Compilation Compilation;

		public abstract IReadOnlyList<DocumentId> GetDocumentsIdsToModify();

		protected SyntaxRewriter(Code code)
		{
			Solution = code.Solution;
			Compilation = code.Compilation;
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
