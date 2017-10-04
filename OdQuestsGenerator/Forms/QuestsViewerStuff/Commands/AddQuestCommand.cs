using System.IO;
using Dataweb.NShape;
using Microsoft.CodeAnalysis.CSharp;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;
using OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Commands
{
	class AddQuestCommand : Command
	{
		private readonly Quest quest;
		private readonly Sector sector;
		private readonly CodeBulk codeBulk;
		private readonly Shape shape;

		public AddQuestCommand(Quest quest, Sector sector, Shape shape, EditingContext context)
			: base(context)
		{
			this.quest = quest;
			this.sector = sector;
			this.shape = shape;

			codeBulk = CreateCode(quest, sector, Context);
		}

		public override void Do()
		{
			Context.Code.Add(codeBulk);
			Context.Code.QuestsAndCodeBulks[quest] = codeBulk;
			Context.Flow.Graph.AddNode(quest);
			sector.Quests.Add(quest);

			codeBulk.WasModified = true;

			var n = Context.Flow.Graph.FindNodeForQuest(quest);
			if (shape.Diagram != null) {
				Context.FlowView.RegisterShapeForNode(n, shape);
			} else {
				Context.FlowView.AddShapeForNode(n, shape);
			}
		}

		public override void Undo()
		{
			Context.Code.Remove(codeBulk);
			Context.Code.QuestsAndCodeBulks.Remove(quest);
			Context.Flow.Graph.RemoveNode(quest);
			sector.Quests.Remove(quest);

			Context.FlowView.RemoveNodeShape(shape);
		}

		private static CodeBulk CreateCode(Quest quest, Sector sector, EditingContext context)
		{
			var path = Path.Combine(context.Code.PathToProject, "Sectors", sector.Name, "Quests", CodeEditor.FormatQuestNameToFileName(quest.Name));
			var tree = SyntaxFactory.ParseSyntaxTree(ToCodeTransformer.Transform(quest));

			return new CodeBulk(CodeBulkType.Quest, tree, path);
		}
	}
}
