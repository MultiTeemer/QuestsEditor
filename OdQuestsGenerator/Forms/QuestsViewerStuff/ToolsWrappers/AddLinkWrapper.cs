using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataweb.NShape;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.CodeReaders;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class AddLinkWrapper : ToolWrapper
	{
		public AddLinkWrapper(EditingContext context)
			: base(context)
		{}

		public override void OnShapesInserted(List<Shape> affectedShapes)
		{
			base.OnShapesInserted(affectedShapes);

			if (affectedShapes.Count == 1 && affectedShapes.First() is Polyline) {
				var p = affectedShapes.First() as Polyline;
				var cps = p.GetControlPointIds(ControlPointCapabilities.All);
				var p1 = p.GetControlPointPosition(cps.First());
				var p2 = p.GetControlPointPosition(cps.Last());

				Box b1 = null;
				Box b2 = null;

				foreach (var s in p.Diagram.Shapes.Except(p)) {
					if (s.ContainsPoint(p1.X, p1.Y)) {
						b1 = s as Box;
					} else if (s.ContainsPoint(p2.X, p2.Y)) {
						b2 = s as Box;
					}
				}

				if (b1 != null && b2 != null && b1 != b2) {
					var n1 = Context.FlowView.GetNodeForShape(b1);
					var n2 = Context.FlowView.GetNodeForShape(b2);
					var link = new Link(n1, n2);

					if (!n2.Quest.IsLinksToEditable()) {
						DeleteShapeOnDeselect(p);
						var msg = $"Couldn't create link to {n2.Quest.Name} quest - links to this quest are editable only through code.";
						MessageBox.Show(msg);
						return;
					}

					if (Context.Flow.Graph.ExistsLink(link)) {
						DeleteShapeOnDeselect(p);
						var msg = $"Couldn't create link from {n1.Quest.Name} to {n2.Quest.Name} quest - link already exists.";
						MessageBox.Show(msg);
						return;
					}

					Context.FlowView.RegisterShapeForLink(p, link);

					if (!n1.Quest.IsActive() || !n2.Quest.IsActive()) {
						var command = new CompositeCommand(Context);
						if (!n1.Quest.IsActive()) {
							command.AddCommand(new ActivateQuestCommand(n1.Quest, Context.Flow.GetSectorForQuest(n1.Quest), Context));
						}
						if (!n2.Quest.IsActive()) {
							command.AddCommand(new ActivateQuestCommand(n2.Quest, Context.Flow.GetSectorForQuest(n2.Quest), Context));
						}
						command.AddCommand(new AddLinkCommand(link, Context));

						Context.History.Do(command);
					} else {
						Context.History.Do(new AddLinkCommand(link, Context));
					}
				} else {
					DeleteShapeOnDeselect(p);
				}
			} else {
				DeleteShapesOnDeselect(affectedShapes);
			}
		}
	}
}
