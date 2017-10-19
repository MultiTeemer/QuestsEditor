using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.GeneralShapes;
using Dataweb.NShape.WinFormsUI;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestFlowViewerStuff
{
	class QuestFlowView : DiagramWrapper
	{
		private const int ShapeShiftX = 20;
		private const int ShapeWidth = 800;
		private const int ShapeHeight = 100;
		private const int BorderShapeHeight = 20;
		private const int Gap = 50;

		public readonly TwoWayDictionary<QuestAction, Shape> ActionsAndShapes = new TwoWayDictionary<QuestAction, Shape>();

		private readonly List<Shape> borderShapes = new List<Shape>();

		public Quest CurrentQuest { get; private set; }

		public QuestFlowView(DiagramSetController controller, Project project, Display display)
			: base(controller, project, display)
		{}

		public void DisplayFlow(Quest quest)
		{
			CurrentQuest = quest;

			InitDiagram(new Size(1000, 5000));
			InitShapes(quest);

			ArrangeShapes();
		}

		public void AddActionShape(QuestAction action, Shape shape)
		{
			ActionsAndShapes.Add(action, shape);
			AddShape(shape);
		}

		public void RemoveActionShape(Shape shape)
		{
			ActionsAndShapes.Remove(shape);
			RemoveShape(shape);
		}

		public void ArrangeShapes()
		{
			ClearBorderShapes();

			var curY = 0;
			for (int i = 0; i < CurrentQuest.States.Count; ++i) {
				var actions = CurrentQuest.States[i].Ensure<QuestActionsData>().Actions;
				foreach (var action in actions) {
					var shape = ActionsAndShapes[action];
					shape.X = ShapeWidth / 2 + ShapeShiftX;
					shape.Y = curY + ShapeHeight / 2 + Gap;

					curY += ShapeHeight + Gap;
				}

				if (i != CurrentQuest.States.Count - 1) {
					InitStateBorderShape(ref curY);
				}
			}

			Diagram.Height = GetBottomOfAllShapes() + ShapeHeight / 2 + Gap;
		}

		private void InitShapes(Quest quest)
		{
			ActionsAndShapes.Clear();

			foreach (var state in quest.States) {
				InitShapes(state);
			}
		}

		private void InitShapes(State state)
		{
			foreach (var action in state.Ensure<QuestActionsData>().Actions) {
				var shape = (Box)Project.ShapeTypes["Box"].CreateInstance();
				shape.Width = ShapeWidth;
				shape.Height = ShapeHeight;

				shape.SetCaptionText(0, QuestActionPresenter.GetPresentation(action));

				ActionsAndShapes.Add(action, shape);

				Diagram.Shapes.Add(shape);
				Project.Repository.Insert((Shape)shape, Diagram);
			}
		}

		private void InitStateBorderShape(ref int y)
		{
			var shape = (Box)Project.ShapeTypes["Box"].CreateInstance();
			shape.Width = ShapeWidth;
			shape.Height = BorderShapeHeight;
			shape.X = ShapeWidth / 2 + ShapeShiftX;
			shape.Y = y + BorderShapeHeight / 2 + Gap;
			shape.FillStyle = new FillStyle("", new ColorStyle("", Color.Orange), new ColorStyle("", Color.Orange));

			Diagram.Shapes.Add(shape);
			Project.Repository.Insert((Shape)shape, Diagram);

			borderShapes.Add(shape);

			y += ShapeHeight / 2 + Gap;
		}

		private void ClearBorderShapes()
		{
			foreach (var bs in borderShapes) {
				RemoveShape(bs);
			}
			borderShapes.Clear();
		}

		private int GetBottomOfAllShapes() => Diagram.Shapes.Count == 0 ? 0 : Diagram.Shapes.Select(s => s.Y).Max();
	}
}
