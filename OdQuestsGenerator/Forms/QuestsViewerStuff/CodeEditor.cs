using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Rename;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
{
	class CodeEditor
	{
		private readonly Code code;
		private readonly List<Document> documents = new List<Document>();
		private readonly Dictionary<CodeBulk, Document> codeBulk2document = new Dictionary<CodeBulk, Document>();
		private readonly Dictionary<Document, CodeBulk> document2codeBulk = new Dictionary<Document, CodeBulk>();

		private Solution solution;
		private Compilation compilation;

		public CodeEditor(Code code)
		{
			this.code = code;
		}

		public void Initialize()
		{
			BuildSolution();
		}

		public CodeBulk Rename(CodeBulk codeBulk, SyntaxNode decl, string newName)
		{
			var doc = codeBulk2document[codeBulk];
			var sol = solution;
			var syntaxTree = doc.GetSyntaxTreeAsync().Result;
			var model = compilation.GetSemanticModel(syntaxTree);
			var locDecl = syntaxTree.GetRoot().DescendantNodesAndSelf().First(n => n.IsEquivalentTo(decl));
			var symbol = model.GetDeclaredSymbol(locDecl);

			var options = solution.Workspace.Options;

			var refs = SymbolFinder
				.FindReferencesAsync(symbol, solution)
				.Result;

			var modifiedDocs = SymbolFinder
				.FindReferencesAsync(symbol, solution)
				.Result
				.SelectMany(r => r.Locations.Select(l => l.Document))
				.ToList();
			modifiedDocs.Add(doc);

			sol = Renamer.RenameSymbolAsync(sol, symbol, newName, null).Result;

			CodeBulk res = null;
			for (int i = 0; i < documents.Count; ++i) {
				var oldDoc = documents[i];
				var cb = document2codeBulk[oldDoc];
				var newDoc = sol.Projects.First().Documents.First(d => d.Name == oldDoc.Name);

				if (document2codeBulk[oldDoc] == codeBulk) {
					res = cb;
				}
				if (modifiedDocs.Any(d => d.Name == oldDoc.Name)) {
					cb.Tree = newDoc.GetSyntaxTreeAsync().Result;
				}
				documents[i] = newDoc;
				codeBulk2document[cb] = newDoc;
				document2codeBulk[newDoc] = cb;
				document2codeBulk.Remove(oldDoc);
			}

			solution = sol;
			compilation = solution.Projects.First().GetCompilationAsync().Result;

			return res;
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
				var d = project.AddDocument(code.GetPathToFile(cb), cb.Tree.GetRoot());
				project = d.Project;

				codeBulk2document[cb] = d;
				document2codeBulk[d] = cb;
				documents.Add(d);
			}

			solution = project.Solution;
			compilation = project.GetCompilationAsync().Result;
		}
	}
}
