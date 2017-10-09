using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Data
{
	enum CodeBulkType
	{
		Quest,
		Sector,
		Config,
	}

	class CodeBulk
	{
		public readonly CodeBulkType Type;
		public readonly string PathToFile;

		private SyntaxTree tree;

		public SyntaxTree Tree
		{
			get => tree;
			set
			{
				if (value != tree) {
					var oldValue = tree;
					tree = value;
					WasModified = true;

					TreeUpdated?.Invoke(this, oldValue);
				}
			}
		}
	
		public bool WasModified { get; set; }

		public event Action<CodeBulk, SyntaxTree> TreeUpdated;

		public CodeBulk(CodeBulkType type, SyntaxTree tree, string pathToFile)
		{
			Type = type;
			Tree = tree;
			PathToFile = pathToFile;

			WasModified = false;
		}
	}

	class Code
	{
		public string PathToProject { get; set; }
		public readonly TwoWayDictionary<CodeBulk, DocumentId> CodeBulksAndDocumentsIds = new TwoWayDictionary<CodeBulk, DocumentId>();
		public readonly TwoWayDictionary<Quest, CodeBulk> QuestsAndCodeBulks = new TwoWayDictionary<Quest, CodeBulk>();
		public readonly TwoWayDictionary<Sector, CodeBulk> SectorsAndCodeBulks = new TwoWayDictionary<Sector, CodeBulk>();

		private readonly List<CodeBulk> codeBulks = new List<CodeBulk>();
		private readonly List<CodeBulk> bulksToDelete = new List<CodeBulk>();
		private readonly List<CodeBulk> bulksToAdd = new List<CodeBulk>();
		private readonly List<string> pathsToProjectFiles = new List<string>();
		private readonly Dictionary<string, CodeBulk> fileToCodeBulk = new Dictionary<string, CodeBulk>();

		public IReadOnlyList<CodeBulk> AllCode => codeBulks;
		public IReadOnlyList<CodeBulk> SectorsCode => CodeBulksOfType(CodeBulkType.Sector).ToList();
		public IReadOnlyList<CodeBulk> QuestsCode => CodeBulksOfType(CodeBulkType.Quest).ToList();
		public IReadOnlyList<CodeBulk> ConfigsCode => CodeBulksOfType(CodeBulkType.Config).ToList();

		public Solution Solution { get; private set; }
		public Compilation Compilation { get; private set; }

		public event Action Saved;

		public Code()
		{
			BuildSolution();
		}

		public void AddPathToProjectFile(string path)
		{
			pathsToProjectFiles.Add(path);
		}

		public CodeBulk ReadFromFile(string path, CodeBulkType type)
		{
			return !fileToCodeBulk.ContainsKey(path)
				? ReadFromFileAndCache(path, type)
				: fileToCodeBulk[path];
		}

		public void Save()
		{
			foreach (var projPath in pathsToProjectFiles) {
				var workingDir = Path.GetDirectoryName(projPath);

				var proj = new XmlDocument();
				proj.Load(projPath);

				foreach (var cb in bulksToAdd) {
					AddFileToProject(cb.PathToFile, workingDir, proj);
				}

				foreach (var cb in bulksToDelete) {
					RemoveFileFromProject(cb.PathToFile, proj);
				}

				using (var writer = new XmlTextWriter(projPath, new UTF8Encoding(false))) {
					writer.Formatting = Formatting.Indented;
					proj.Save(writer);
				}
			}

			bulksToAdd.Clear();
			foreach (var cb in bulksToDelete) {
				File.Delete(cb.PathToFile);
			}
			bulksToDelete.Clear();

			var modifiedCodeBulks = codeBulks.Where(cb => cb.WasModified);
			foreach (var codeBulk in modifiedCodeBulks) {
				WriteCodeToFile(codeBulk.Tree, codeBulk.PathToFile);
				codeBulk.WasModified = false;
			}

			Saved?.Invoke();
		}

		public CodeBulk RenameFile(CodeBulk codeBulk, string newFileName)
		{
			var newPath = Path.Combine(Path.GetDirectoryName(codeBulk.PathToFile), newFileName);
			var newCb = new CodeBulk(codeBulk.Type, codeBulk.Tree, newPath);
			newCb.WasModified = true;

			Quest quest = null;
			Sector sector = null;
			if (QuestsAndCodeBulks.Contains(codeBulk)) {
				quest = QuestsAndCodeBulks[codeBulk];
			} else if (SectorsAndCodeBulks.Contains(codeBulk)) {
				sector = SectorsAndCodeBulks[codeBulk];
			}

			Remove(codeBulk);
			Add(newCb);

			if (quest != null) {
				QuestsAndCodeBulks[quest] = newCb;
			} else if (sector != null) {
				SectorsAndCodeBulks[sector] = newCb;
			}

			return newCb;
		}

		public void Add(CodeBulk codeBulk, bool newFile = true)
		{
			codeBulks.Add(codeBulk);
			fileToCodeBulk.Add(codeBulk.PathToFile, codeBulk);
			if (newFile) {
				bulksToAdd.Add(codeBulk);
			}

			var project = Solution.Projects.First();
			var doc = project.AddDocument(codeBulk.PathToFile, codeBulk.Tree.GetRoot());
			CodeBulksAndDocumentsIds[codeBulk] = doc.Id;

			codeBulk.TreeUpdated += CodeBulk_TreeUpdated;

			SetSolution(doc.Project.Solution);
		}

		public void Remove(CodeBulk codeBulk)
		{
			codeBulks.Remove(codeBulk);
			bulksToDelete.Add(codeBulk);
			fileToCodeBulk.Remove(codeBulk.PathToFile);

			var docId = CodeBulksAndDocumentsIds[codeBulk];
			CodeBulksAndDocumentsIds.Remove(codeBulk);

			if (codeBulk.Type == CodeBulkType.Quest) {
				QuestsAndCodeBulks.Remove(codeBulk);
			} else if (codeBulk.Type == CodeBulkType.Sector) {
				SectorsAndCodeBulks.Remove(codeBulk);
			}

			codeBulk.TreeUpdated -= CodeBulk_TreeUpdated;

			SetSolution(Solution.RemoveDocument(docId));
		}

		public IReadOnlyList<CodeBulk> CodeBulksOfTypes(params CodeBulkType[] types) =>
			types.SelectMany(type => CodeBulksOfType(type)).ToList();

		public void SetSolution(Solution solution)
		{
			Solution = solution;
			Compilation = solution.Projects.First().GetCompilationAsync().Result;
		}

		public ISymbol GetSymbolFor(SyntaxNode node, CodeBulk containingCode)
		{
			var docId = CodeBulksAndDocumentsIds[containingCode];
			var syntaxTree = Solution.GetDocument(docId).GetSyntaxTreeAsync().Result;
			var model = Compilation.GetSemanticModel(syntaxTree);
			var locDecl = syntaxTree.GetRoot().DescendantNodesAndSelf().First(n => n.IsEquivalentTo(node, topLevel: true));

			return model.GetDeclaredSymbol(locDecl);
		}

		public Document GetMappedCode(CodeBulk codeBulk)
		{
			return Solution.GetDocument(CodeBulksAndDocumentsIds[codeBulk]);
		}

		public CodeBulk GetCodeBulkByTreeFromSolution(SyntaxTree treeInSolution)
		{
			var doc = Solution.Projects.SelectMany(p => p.Documents).FirstOrDefault(d => d.GetSyntaxTreeAsync().Result == treeInSolution);
			if (doc == null) return null;

			return CodeBulksAndDocumentsIds[doc.Id];
		}

		private void AddFileToProject(string filePath, string pathToParentDirectory, XmlDocument proj)
		{
			var compileGroup = GetCompileGroup(proj);
			var item = proj.CreateElement("Compile", proj["Project"].NamespaceURI);
			var include = item.Attributes.Append(proj.CreateAttribute("Include"));
			include.Value = filePath.Replace(pathToParentDirectory, "").TrimStart(Path.DirectorySeparatorChar);
			compileGroup.AppendChild(item);
		}

		private void RemoveFileFromProject(string filePath, XmlDocument proj)
		{
			var compileGroup = GetCompileGroup(proj);
			var itemToRemove = compileGroup.ChildNodes.Cast<XmlNode>().Where(n => n.Name == "Compile")
				.FirstOrDefault(n => filePath.EndsWith(n.Attributes["Include"].Value));
			compileGroup.RemoveChild(itemToRemove);
		}

		private XmlNode GetCompileGroup(XmlDocument proj)
		{
			var doc = proj["Project"];
			var nodes = doc.ChildNodes.Cast<XmlNode>().Where(n => n.Name == "ItemGroup").ToList();
			return nodes[1];
		}

		private void WriteCodeToFile(SyntaxTree tree, string filePath)
		{
			using (var writer = new StreamWriter(File.OpenWrite(filePath))) {
				var workspace = new AdhocWorkspace();
				var res = Formatter.Format(tree.GetRoot(), workspace, GetFormattingOptions(workspace));
				writer.WriteLine(res);
			}
		}

		private CodeBulk ReadFromFileAndCache(string path, CodeBulkType type)
		{
			var tree = FileSystem.ReadCodeFromFile(path);
			var bulk = new CodeBulk(type, tree, path);

			Add(bulk, newFile: false);

			return bulk;
		}

		private IEnumerable<CodeBulk> CodeBulksOfType(CodeBulkType type) => codeBulks.Where(cb => cb.Type == type);

		private OptionSet GetFormattingOptions(Workspace workspace) =>
			workspace.Options
			.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInTypes, true)
			.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInControlBlocks, false)
			.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInLambdaExpressionBody, false)
			.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInObjectCollectionArrayInitializers, false)
			.WithChangedOption(CSharpFormattingOptions.NewLineForElse, false)
			.WithChangedOption(CSharpFormattingOptions.NewLineForCatch, false)
			.WithChangedOption(CSharpFormattingOptions.NewLineForFinally, false)
			.WithChangedOption(new OptionKey(FormattingOptions.UseTabs, "C#"), true);

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

			SetSolution(project.Solution);
		}

		private void UpdateMappedDocument(CodeBulk codeBulk)
		{
			var docId = CodeBulksAndDocumentsIds[codeBulk];
			SetSolution(Solution.WithDocumentSyntaxRoot(docId, codeBulk.Tree.GetRoot()));
		}

		private void CodeBulk_TreeUpdated(CodeBulk codeBulk, SyntaxTree oldTree)
		{
			UpdateMappedDocument(codeBulk);
		}
	}
}
