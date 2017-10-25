using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataweb.NShape;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.CodeReaders;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class AddLinkWrapper : ToolWrapper<LinearShapeCreationTool, FlowView>
	{
		public AddLinkWrapper(EditingContext context, LinearShapeCreationTool tool, FlowView view)
			: base(context, tool, view)
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
					var n1 = DiagramWrapper.GetNodeForShape(b1);
					var n2 = DiagramWrapper.GetNodeForShape(b2);
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

					DiagramWrapper.RegisterShapeForLink(p, link);

					if (!n1.Quest.IsActive() || !n2.Quest.IsActive()) {
						var command = new CompositeCommand(Context);

						void InitQuestActivationCommand(Quest quest) =>
							command.AddCommand(
								CommandsCreation.ActivateQuest(
									quest,
									Context.Flow.GetSectorForQuest(quest),
									Context,
									DiagramWrapper
								)
							);

						if (!n1.Quest.IsActive()) {
							InitQuestActivationCommand(n1.Quest);
						}
						if (!n2.Quest.IsActive()) {
							InitQuestActivationCommand(n2.Quest);
						}
						command.AddCommand(CommandsCreation.AddLink(link, Context, DiagramWrapper));

						Context.History.Do(command);
					} else {
						Context.History.Do(CommandsCreation.AddLink(link, Context, DiagramWrapper));
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
