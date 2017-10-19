using System.Collections.Generic;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.Controllers;

namespace OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing
{
	class ToolsManager
	{
		private readonly DiagramWrapper flowView;
		private readonly ToolSetController toolSetController;
		private readonly Dictionary<Tool, IToolWrapper> toolSet = new Dictionary<Tool, IToolWrapper>();

		public IToolWrapper CurrentActiveToolWrapper { get; private set; }
		public Tool CurrentActiveTool => toolSetController.SelectedTool;

		public ToolsManager(ToolSetController toolSetController, DiagramWrapper flowView)
		{
			this.flowView = flowView;
			this.toolSetController = toolSetController;

			toolSetController.ToolSelected += ToolSetController_ToolSelected;
			toolSetController.Project.Repository.ShapesInserted += Repository_ShapesInserted;
			toolSetController.Project.Repository.ShapesUpdated += Repository_ShapesUpdated;
			toolSetController.Project.Repository.ShapesDeleted += Repository_ShapesDeleted;

			flowView.Display.ShapeClick += Display_ShapeClick;
			flowView.Display.ShapeDoubleClick += Display_ShapeDoubleClick;
			flowView.Display.ShapeMoved += Display_ShapeMoved;
		}

		private void Display_ShapeMoved(object sender, DiagramPresenterShapeEventArgs e)
		{
			if (e.Shape != null) {
				CurrentActiveToolWrapper?.OnShapeMoved(e.Shape);
			}
		}

		private void Display_ShapeDoubleClick(object sender, DiagramPresenterShapeClickEventArgs e)
		{
			if (e.Shape != null) {
				CurrentActiveToolWrapper?.OnShapeDoubleClick(e.Shape);
			}
		}

		private void Display_ShapeClick(object sender, DiagramPresenterShapeClickEventArgs e)
		{
			if (e.Shape != null) {
				CurrentActiveToolWrapper?.OnShapeClick(e.Shape);
			}
		}

		private void Repository_ShapesDeleted(object sender, RepositoryShapesEventArgs e)
		{
			if (!flowView.DiagramEdited && e.Count > 0) {
				CurrentActiveToolWrapper?.OnShapesDeleted(e.Shapes.ToList());
			}
		}

		private void Repository_ShapesUpdated(object sender, RepositoryShapesEventArgs e)
		{
			if (!flowView.DiagramEdited && e.Count > 0) {
				CurrentActiveToolWrapper?.OnShapesUpdated(e.Shapes.ToList());
			}
		}

		private void Repository_ShapesInserted(object sender, RepositoryShapesEventArgs e)
		{
			if (!flowView.DiagramEdited && e.Count > 0) {
				CurrentActiveToolWrapper?.OnShapesInserted(e.Shapes.ToList());
			}
		}

		private void ToolSetController_ToolSelected(object sender, ToolEventArgs e)
		{
			if (e.Tool != null && toolSet.ContainsKey(e.Tool)) {
				ActivateTool(e.Tool);
			}
		}

		public void AddTool(Tool tool, IToolWrapper toolWrapper)
		{
			toolSet[tool] = toolWrapper;
			toolSetController.AddTool(tool);
		}

		public void Clear()
		{
			toolSetController.Clear();
			toolSet.Clear();
		}

		private void ActivateTool(Tool tool)
		{
			CurrentActiveToolWrapper?.OnToolDeselected();

			CurrentActiveToolWrapper = toolSet[tool];

			CurrentActiveToolWrapper.OnToolSelected();
		}
	}
}
