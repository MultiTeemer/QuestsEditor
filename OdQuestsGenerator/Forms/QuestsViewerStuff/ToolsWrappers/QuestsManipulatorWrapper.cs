﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataweb.NShape;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataValidators;
using OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class QuestsManipulatorWrapper : ToolWrapper<BaseUIStuff.OverloadedTools.SelectionTool, FlowView>
	{
		public QuestsManipulatorWrapper(EditingContext context, BaseUIStuff.OverloadedTools.SelectionTool tool, FlowView view)
			: base(context, tool, view)
		{}

		public override void OnShapesUpdated(List<Shape> affectedShapes)
		{
			base.OnShapesUpdated(affectedShapes);

			var shape = affectedShapes.First() as Box;
			if (shape != null) {
				TryToInitializeRenameCommand(shape);
			}
		}

		public override void OnShapesDeleted(List<Shape> affectedShapes)
		{
			base.OnShapesDeleted(affectedShapes);

			var questsToDelete = affectedShapes.OfType<Box>().Select(FindQuestForShape).ToList();
			if (questsToDelete.Count > 0) {
				var command = new CompositeCommand();
				foreach (var q in questsToDelete) {
					var links = Context.Flow.Graph.GetLinksForNode(Context.Flow.Graph.FindNodeForQuest(q));
					command.AddCommands(links.Select(l => CommandsCreation.RemoveLink(l, Context, DiagramWrapper)));
				}
				command.AddCommands(questsToDelete.Select(
					q => CommandsCreation.DeactivateQuest(q, Context, DiagramWrapper)
				));
				command.AddCommands(questsToDelete.Select(
					q => CommandsCreation.RemoveQuest(
						q,
						Context,
						Context.Flow.Graph.FindNodeForQuest(q),
						DiagramWrapper,
						DiagramWrapper.GetShapeForQuest(q))
					)
				);
				Context.History.Do(command);
			} else {
				var commands = affectedShapes.Select(FindLinkForShape)
					.Where(l => l.HasValue)
					.Select(l => CommandsCreation.RemoveLink(l.Value, Context, DiagramWrapper))
					.ToList();
				if (commands.Count > 0) {
					Context.History.Do(new CompositeCommand(commands));
				}
			}
		}

		public override void OnKeyUp(Keys keys)
		{
			base.OnKeyUp(keys);

			if (keys == Keys.Enter && Tool.LastPerformedAction == BaseUIStuff.OverloadedTools.SelectionTool.EditAction.EditCaption) {
				var shape = Tool.LastTouchedShape as Box;
				if (shape != null) {
					TryToInitializeRenameCommand(shape);
					(DiagramWrapper.Display as IDiagramPresenter).CloseCaptionEditor(false);
				}
			}
		}

		public override void OnShapeDoubleClick(Shape shape)
		{
			base.OnShapeDoubleClick(shape);

			var box = shape as Box;
			if (box != null) {
				var form = new QuestFlowViewer(FindQuestForShape(box), Context);
				form.Show();
				form.Shown += (s, a) => form.Focus();
			}
		}

		private void TryToInitializeRenameCommand(Box shape)
		{
			var quest = FindQuestForShape(shape);
			if (quest.Name != shape.Text) {
				var newName = Regex.Replace(shape.Text, "\\s+", "");
				if (IsItOkForCodeGeneration.Check(newName)) {
					Context.History.Do(CommandsCreation.RenameQuest(quest, quest.Name, newName, Context, DiagramWrapper, shape));
				} else {
					shape.SetCaptionText(0, quest.Name);
				}
			}
		}

		private Quest FindQuestForShape(Shape shape)
		{
			return DiagramWrapper.GetNodeForShape(shape)?.Quest;
		}

		private Link? FindLinkForShape(Shape shape)
		{
			return DiagramWrapper.GetLinkForShape(shape);
		}
	}
}
