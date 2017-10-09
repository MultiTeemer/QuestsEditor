using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;

namespace OdQuestsGenerator.Commands
{
	class AddQuestCommand : Command
	{
		private readonly Quest quest;
		private readonly Sector sector;
		private readonly CodeBulk codeBulk;

		public AddQuestCommand(Quest quest, Sector sector, EditingContext context)
			: base(context)
		{
			this.quest = quest;
			this.sector = sector;

			codeBulk = CreateCode(quest, sector, Context);
		}

		public override void Do()
		{
			Context.Code.Add(codeBulk);
			Context.Code.QuestsAndCodeBulks[quest] = codeBulk;
			Context.Flow.Graph.AddNode(quest);
			sector.Quests.Add(quest);

			codeBulk.WasModified = true;
		}

		public override void Undo()
		{
			Context.Code.Remove(codeBulk);
			Context.Code.QuestsAndCodeBulks.Remove(quest);
			Context.Flow.Graph.RemoveNode(quest);
			sector.Quests.Remove(quest);
		}

		private static CodeBulk CreateCode(Quest quest, Sector sector, EditingContext context)
		{
			var path = Path.Combine(context.Code.PathToProject, "Sectors", sector.Name, "Quests", CodeEditor.FormatQuestNameToFileName(quest.Name));
			var tree = SyntaxFactory.ParseSyntaxTree(ToCodeTransformer.Transform(quest));

			return new CodeBulk(CodeBulkType.Quest, tree, path);
		}
	}
}
