using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Rename;
using OdQuestsGenerator.Forms.QuestsViewerStuff.SyntaxRewriters;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
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
		private readonly TwoWayDictionary<CodeBulk, DocumentId> codeBulksAndDocumentsIds = new TwoWayDictionary<CodeBulk, DocumentId>();

		public Solution Solution { get; private set; }
		public Compilation Compilation { get; private set; }

		public CodeEditor(Code code)
		{
			this.code = code;
		}

		public void Initialize()
		{
			BuildSolution();
		}

		public void Add(CodeBulk codeBulk)
		{
			code.Add(codeBulk);

			var project = Solution.Projects.First();
			var doc = project.AddDocument(codeBulk.PathToFile, codeBulk.Tree.GetRoot());
			codeBulksAndDocumentsIds[codeBulk] = doc.Id;

			SetSolution(doc.Project.Solution);
		}

		public void Remove(CodeBulk codeBulk)
		{
			code.Remove(codeBulk);

			var doc = codeBulksAndDocumentsIds[codeBulk];
			codeBulksAndDocumentsIds.Remove(codeBulk);

			SetSolution(Solution.RemoveDocument(doc));
		}

		public void Rename(CodeBulk codeBulk, SyntaxNode decl, string newName)
		{
			var docId = codeBulksAndDocumentsIds[codeBulk];
			var doc = Solution.GetDocument(docId);
			var syntaxTree = doc.GetSyntaxTreeAsync().Result;
			var model = Compilation.GetSemanticModel(syntaxTree);
			var locDecl = syntaxTree.GetRoot().DescendantNodesAndSelf().First(n => n.IsEquivalentTo(decl));
			var symbol = model.GetDeclaredSymbol(locDecl);

			var options = Solution.Workspace.Options;

			var refs = SymbolFinder
				.FindReferencesAsync(symbol, Solution)
				.Result;

			var modifiedDocs = SymbolFinder
				.FindReferencesAsync(symbol, Solution)
				.Result
				.SelectMany(r => r.Locations.Select(l => l.Document))
				.ToList();
			modifiedDocs.Add(doc);

			var newSolution = Renamer.RenameSymbolAsync(Solution, symbol, newName, null).Result;

			foreach (var d in modifiedDocs) {
				var cb = codeBulksAndDocumentsIds[d.Id];
				cb.Tree = newSolution.GetDocument(d.Id).GetSyntaxTreeAsync().Result;
			}

			SetSolution(newSolution);
		}

		public ISymbol GetSymbolFor(SyntaxNode node, CodeBulk containingCode)
		{
			var docId = codeBulksAndDocumentsIds[containingCode];
			var syntaxTree = Solution.GetDocument(docId).GetSyntaxTreeAsync().Result;
			var model = Compilation.GetSemanticModel(syntaxTree);
			var locDecl = syntaxTree.GetRoot().DescendantNodesAndSelf().First(n => n.IsEquivalentTo(node, topLevel: true));

			return model.GetDeclaredSymbol(locDecl);
		}

		public CodeSnapshot ApplySyntaxRewriters(params SyntaxRewriter[] rewriters)
		{
			return ApplySyntaxRewriters(rewriters.ToList());
		}

		public CodeSnapshot ApplySyntaxRewriters(List<SyntaxRewriter> rewriters)
		{
			var newSolution = Solution;

			var res = new CodeSnapshot();
			var allDocsIdsToModify = rewriters.SelectMany(r => r.GetDocumentsIdsToModify()).ToList();
			foreach (var id in allDocsIdsToModify) {
				var cb = codeBulksAndDocumentsIds[id];
				res.PreviousCode[cb] = cb.Tree;
			}

			foreach (var rewriter in rewriters) {
				var docsToModify = rewriter.GetDocumentsIdsToModify();
				var project = Solution.Projects.First();
				foreach (var id in docsToModify) {
					var syntaxRoot = rewriter.Visit(Solution.GetDocument(id).GetSyntaxRootAsync().Result);
					codeBulksAndDocumentsIds[id].Tree = syntaxRoot.SyntaxTree;
					newSolution = newSolution.WithDocumentSyntaxRoot(id, syntaxRoot);
				}
			}

			SetSolution(newSolution);

			return res;
		}

		public void ApplySnapshot(CodeSnapshot snapshot)
		{
			var newSolution = Solution;

			foreach (var kv in snapshot.PreviousCode) {
				kv.Key.Tree = kv.Value;
				newSolution = newSolution.WithDocumentSyntaxRoot(codeBulksAndDocumentsIds[kv.Key], kv.Value.GetRoot());
			}

			SetSolution(newSolution);
		}

		private void BuildSolution()
		{
			var ws = new AdhocWorkspace();
			var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
			var references = new List<MetadataReference>() { Mscorlib };
			var projInfo = ProjectInfo.Create(
				ProjectId.CreateNewId(),
				VersionStamp.Default,
				"MyProject",
				"MyAssembly",
				"C#",
				metadataReferences: references
			);
			var project = ws.AddProject(projInfo);

			foreach (var cb in code.AllCode) {
				var d = project.AddDocument(cb.PathToFile, cb.Tree.GetRoot());
				project = d.Project;
				codeBulksAndDocumentsIds[d.Id] = cb;
			}

			SetSolution(project.Solution);
		}

		private void SetSolution(Solution solution)
		{
			Solution = solution;
			Compilation = solution.Projects.First().GetCompilationAsync().Result;
		}
	}
}
