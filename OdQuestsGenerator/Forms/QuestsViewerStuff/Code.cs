using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
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
		public readonly SyntaxTree Tree;
	
		public bool WasModified { get; set; }

		public CodeBulk(CodeBulkType type, SyntaxTree tree)
		{
			Type = type;
			Tree = tree;

			WasModified = false;
		}
	}

	class Code
	{
		private readonly List<CodeBulk> codeBulks = new List<CodeBulk>();
		private readonly Dictionary<string, CodeBulk> fileToCodeBulk = new Dictionary<string, CodeBulk>();

		public IReadOnlyList<CodeBulk> AllCode => codeBulks;

		public CodeBulk ReadFromFile(string path, CodeBulkType type)
		{
			return !fileToCodeBulk.ContainsKey(path)
				? ReadFromFileAndCache(path, type)
				: fileToCodeBulk[path];
		}

		private CodeBulk ReadFromFileAndCache(string path, CodeBulkType type)
		{
			var tree = FileSystem.ReadCodeFromFile(path);
			var bulk = new CodeBulk(type, tree);
			codeBulks.Add(bulk);
			fileToCodeBulk.Add(path, bulk);

			return bulk;
		}
	}
}
