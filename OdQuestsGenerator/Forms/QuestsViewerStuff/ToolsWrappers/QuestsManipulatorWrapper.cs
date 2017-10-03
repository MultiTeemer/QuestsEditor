﻿using System.Collections.Generic;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class QuestsManipulatorWrapper : ToolWrapper
	{
		public QuestsManipulatorWrapper(EditingContext context)
			: base(context)
		{}

		public override void OnShapesInserted(List<Shape> affectedShapes)
		{
			base.OnShapesInserted(affectedShapes);

			if (affectedShapes.Count == 1 && affectedShapes.First() is Polyline) {
				var p = affectedShapes.First() as Polyline;
				
			}
		}

		public override void OnShapesUpdated(List<Shape> affectedShapes)
		{
			base.OnShapesUpdated(affectedShapes);

			if (affectedShapes.Count > 0) {
				var shape = affectedShapes.First() as CaptionedShapeBase;
				Context.History.Do(new RenameQuestCommand(FindQuestForShape(shape), shape.Text, Context));
			}
		}

		public override void OnShapesDeleted(List<Shape> affectedShapes)
		{
			base.OnShapesDeleted(affectedShapes);

			if (affectedShapes.Count > 0) {
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
