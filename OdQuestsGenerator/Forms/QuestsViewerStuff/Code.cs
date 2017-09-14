using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using OdQuestsGenerator.Data;
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
		private readonly Dictionary<Quest, CodeBulk> questToCodeBulk = new Dictionary<Quest, CodeBulk>();
		private readonly Dictionary<Sector, CodeBulk> sectorToCodeBulk = new Dictionary<Sector, CodeBulk>();

		public IReadOnlyList<CodeBulk> AllCode => codeBulks;

		public CodeBulk ReadFromFile(string path, CodeBulkType type)
		{
			return !fileToCodeBulk.ContainsKey(path)
				? ReadFromFileAndCache(path, type)
				: fileToCodeBulk[path];
		}

		public void RegisterQuestforCodeBulk(Quest quest, CodeBulk codeBulk)
		{
			questToCodeBulk[quest] = codeBulk;
		}

		public void RegisterSectorForCodeBulk(Sector sector, CodeBulk codeBulk)
		{
			sectorToCodeBulk[sector] = codeBulk;
		}

		public CodeBulk GetCodeForQuest(Quest quest)
		{
			return questToCodeBulk[quest];
		}

		public CodeBulk GetCodeForSector(Sector sector)
		{
			return sectorToCodeBulk[sector];
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
