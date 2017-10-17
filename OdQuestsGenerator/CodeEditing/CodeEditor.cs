using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Rename;
using OdQuestsGenerator.CodeEditing.SyntaxRewriters;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.CodeEditing
{
	class CodeSnapshot
	{
		public readonly Dictionary<CodeBulk, SyntaxTree> PreviousCode = new Dictionary<CodeBulk, SyntaxTree>();

		public CodeSnapshot Merge(CodeSnapshot snapshot)
		{
			foreach (var kv in snapshot.PreviousCode) {
				if (!PreviousCode.ContainsKey(kv.Key)) {
					PreviousCode[kv.Key] = kv.Value;
				}
			}

			return this;
		}
	}

	class CodeEditor
	{
		private readonly Code code;

		public CodeEditor(Code code)
		{
			this.code = code;
		}

		public void Rename(CodeBulk codeBulk, SyntaxNode decl, string newName)
		{
			var docId = code.CodeBulksAndDocumentsIds[codeBulk];
			var doc = code.Solution.GetDocument(docId);
			var syntaxTree = doc.GetSyntaxTreeAsync().Result;
			var model = code.Compilation.GetSemanticModel(syntaxTree);
			var locDecl = syntaxTree.GetRoot().DescendantNodesAndSelf().First(n => n.IsEquivalentTo(decl));
			var symbol = model.GetDeclaredSymbol(locDecl);

			var options = code.Solution.Workspace.Options;

			var refs = SymbolFinder
				.FindReferencesAsync(symbol, code.Solution)
				.Result;

			var modifiedDocs = SymbolFinder
				.FindReferencesAsync(symbol, code.Solution)
				.Result
				.SelectMany(r => r.Locations.Select(l => l.Document))
				.ToList();
			modifiedDocs.Add(doc);

			var newSolution = Renamer.RenameSymbolAsync(code.Solution, symbol, newName, null).Result;

			foreach (var d in modifiedDocs) {
				var cb = code.CodeBulksAndDocumentsIds[d.Id];
				cb.Tree = newSolution.GetDocument(d.Id).GetSyntaxTreeAsync().Result;
			}

			code.SetSolution(newSolution);
		}

		public ISymbol GetSymbolFor(SyntaxNode node, CodeBulk containingCode)
		{
			var docId = code.CodeBulksAndDocumentsIds[containingCode];
			var syntaxTree = code.Solution.GetDocument(docId).GetSyntaxTreeAsync().Result;
			var model = code.Compilation.GetSemanticModel(syntaxTree);
			var locDecl = syntaxTree.GetRoot().DescendantNodesAndSelf().First(n => n.IsEquivalentTo(node, topLevel: true));

			return model.GetDeclaredSymbol(locDecl);
		}

		public CodeSnapshot ApplySyntaxRewriters(params SyntaxRewriter[] rewriters)
		{
			return ApplySyntaxRewriters(rewriters.ToList());
		}

		public CodeSnapshot ApplySyntaxRewriters(List<SyntaxRewriter> rewriters)
		{
			var newSolution = code.Solution;

			var res = new CodeSnapshot();
			var allDocsIdsToModify = rewriters.SelectMany(r => r.GetDocumentsIdsToModify()).ToList();
			foreach (var id in allDocsIdsToModify) {
				var cb = code.CodeBulksAndDocumentsIds[id];
				res.PreviousCode[cb] = cb.Tree;
			}

			foreach (var rewriter in rewriters) {
				var docsToModify = rewriter.GetDocumentsIdsToModify();
				var newTrees = new Dictionary<DocumentId, SyntaxNode>();
				foreach (var id in docsToModify) {
					newTrees[id] = rewriter.Visit(code.Solution.GetDocument(id).GetSyntaxRootAsync().Result);
				}

				foreach (var id in docsToModify) {
					code.CodeBulksAndDocumentsIds[id].Tree = newTrees[id].SyntaxTree;
					newSolution = newSolution.WithDocumentSyntaxRoot(id, newTrees[id]);
				}
			}

			code.SetSolution(newSolution);

			return res;
		}

		public void ApplySnapshot(CodeSnapshot snapshot)
		{
			var newSolution = code.Solution;

			foreach (var kv in snapshot.PreviousCode) {
				kv.Key.Tree = kv.Value;
				newSolution = newSolution.WithDocumentSyntaxRoot(code.CodeBulksAndDocumentsIds[kv.Key], kv.Value.GetRoot());
			}

			code.SetSolution(newSolution);
		}

		public ISymbol GetQuestClassSymbol(Quest quest)
		{
			var cb = code.QuestsAndCodeBulks[quest];
			var decl = cb.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

			return GetSymbolFor(decl, cb);
		}

		public static string FormatQuestNameForVar(string questName) => $"{char.ToLower(questName[0])}{questName.Substring(1)}Quest";

		public static string FormatQuestNameForClass(string questName) => $"{questName}Quest";

		public static string FromQuestVarNameToQuestName(string varName) => $"{char.ToUpper(varName[0])}{varName.Substring(1, varName.Length - "Quest".Length - 1)}";

		public static string FromQuestClassNametoQuestName(string className) => className.Substring(0, className.Length - "Quest".Length);

		public static string FormatQuestClassNameForVar(string className) => $"{char.ToLower(className[0])}{className.Substring(1)}";

		public static string FormatQuestNameToFileName(string questName) => $"{questName}Quest.cs";
	}
}
