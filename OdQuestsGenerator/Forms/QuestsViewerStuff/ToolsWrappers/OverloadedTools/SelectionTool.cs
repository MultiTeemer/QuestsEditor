using System;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.Controllers;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers.OverloadedTools
{
	class SelectionTool : Dataweb.NShape.SelectionTool
	{
		public enum EditAction
		{
			None = Action.None,
			Select = Action.Select,
			EditCaption = Action.EditCaption,
			MoveHandle = Action.MoveHandle,
			MoveShape = Action.MoveShape,
		}

		public EditAction LastPerformedAction { get; private set; } = EditAction.None;
		public Shape LastTouchedShape { get; private set; }

		private bool wasEnded;

		protected override void StartToolAction(IDiagramPresenter diagramPresenter, int action, MouseState mouseState, bool wantAutoScroll)
		{
			base.StartToolAction(diagramPresenter, action, mouseState, wantAutoScroll);

			if (Enum.IsDefined(typeof(EditAction), action)) {
				LastPerformedAction = (EditAction)action;
				wasEnded = false;
				LastTouchedShape = diagramPresenter.Diagram.Shapes
					.FirstOrDefault(s => s.ContainsPoint(mouseState.X, mouseState.Y));
			} else {
				wasEnded = true;
				LastTouchedShape = null;
			}
		}

		protected override void EndToolAction()
		{
			base.EndToolAction();

			if (!wasEnded) {
				wasEnded = true;
				Console.WriteLine((object)LastTouchedShape ?? "null");
			}
		}
	}
}
