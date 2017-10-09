using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataValidators;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class QuestsManipulatorWrapper : ToolWrapper<OverloadedTools.SelectionTool>
	{
		public QuestsManipulatorWrapper(EditingContext context, OverloadedTools.SelectionTool tool)
			: base(context, tool)
		{}

		public override void OnShapesUpdated(List<Shape> affectedShapes)
		{
			base.OnShapesUpdated(affectedShapes);

			var shape = affectedShapes.First() as CaptionedShapeBase;
			if (shape != null) {
				TryToInitializeRenameCommand(shape);
			}
		}

		public override void OnShapesDeleted(List<Shape> affectedShapes)
		{
			base.OnShapesDeleted(affectedShapes);

			var questsToDelete = affectedShapes.OfType<Box>().Select(FindQuestForShape).ToList();
			if (questsToDelete.Count > 0) {
				var command = new CompositeCommand(Context);
				foreach (var q in questsToDelete) {
					var links = Context.Flow.Graph.GetLinksForNode(Context.Flow.Graph.FindNodeForQuest(q));
					command.AddCommands(links.Select(l => new RemoveLinkCommand(l, Context)));
				}
				command.AddCommands(questsToDelete.Select(q => new RemoveQuestCommand(q, Context)));
				Context.History.Do(command);
			} else {
				var commands = affectedShapes.Select(FindLinkForShape)
					.Where(l => l.HasValue)
					.Select(l => new RemoveLinkCommand(l.Value, Context))
					.ToList();
				if (commands.Count > 0) {
					Context.History.Do(new CompositeCommand(Context, commands));
				}
			}
		}

		public override void OnKeyUp(Keys keys)
		{
			base.OnKeyUp(keys);

			if (keys == Keys.Enter && Tool.LastPerformedAction == OverloadedTools.SelectionTool.EditAction.EditCaption) {
				var shape = Tool.LastTouchedShape as CaptionedShapeBase;
				if (shape != null) {
					TryToInitializeRenameCommand(shape);
					(Context.FlowView.Display as IDiagramPresenter).CloseCaptionEditor(false);
				}
			}
		}

		private void TryToInitializeRenameCommand(CaptionedShapeBase shape)
		{
			var quest = FindQuestForShape(shape);
			if (quest.Name != shape.Text) {
				var newName = Regex.Replace(shape.Text, "\\s+", "");
				if (IsItOkForCodeGeneration.Check(newName)) {
					Context.History.Do(new RenameQuestCommand(quest, newName, Context));
				} else {
					shape.SetCaptionText(0, quest.Name);
				}
			}
		}

		private Quest FindQuestForShape(Shape shape)
		{
			return Context.FlowView.GetNodeForShape(shape)?.Quest;
		}

		private Link? FindLinkForShape(Shape shape)
		{
			return Context.FlowView.GetLinkForShape(shape);
		}
	}
}
