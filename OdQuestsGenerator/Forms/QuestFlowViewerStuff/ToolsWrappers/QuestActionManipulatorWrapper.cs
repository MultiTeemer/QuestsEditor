using System.Collections.Generic;
using System.Drawing;
using Dataweb.NShape;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing;

namespace OdQuestsGenerator.Forms.QuestFlowViewerStuff.ToolsWrappers
{
	class QuestActionManipulatorWrapper : ToolWrapper<BaseUIStuff.OverloadedTools.SelectionTool, QuestFlowView>
	{
		private readonly Dictionary<Shape, Point> shapesPositions = new Dictionary<Shape, Point>();

		public QuestActionManipulatorWrapper(EditingContext context, BaseUIStuff.OverloadedTools.SelectionTool tool, QuestFlowView diagramWrapper)
			: base(context, tool, diagramWrapper)
		{}

		public override void OnToolSelected()
		{
			base.OnToolSelected();

			shapesPositions.Clear();
			
			if (DiagramWrapper.Display.Diagram != null) {
				foreach (var s in DiagramWrapper.Display.Diagram.Shapes) {
					shapesPositions[s] = new Point(s.X, s.Y);
				}
			}
		}

		public override void OnShapesDeleted(List<Shape> affectedShapes)
		{
			base.OnShapesDeleted(affectedShapes);

			foreach (var s in affectedShapes) {
				if (DiagramWrapper.ActionsAndShapes.Contains(s)) {
					InitDeleteQuestCommand(s);
				} else {
					// restore deleted shape in some way
				}
			}
		}

		private void InitDeleteQuestCommand(Shape shape)
		{
			var action = DiagramWrapper.ActionsAndShapes[shape];
			var command = CommandsCreation.RemoveQuestAction(action, DiagramWrapper.CurrentQuest, Context, DiagramWrapper, shape);

			Context.History.Do(command);
		}
	}
}
