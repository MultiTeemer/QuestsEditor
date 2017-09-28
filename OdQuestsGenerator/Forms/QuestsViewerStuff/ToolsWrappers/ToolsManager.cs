using System.Collections.Generic;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.Controllers;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class ToolsManager
	{
		private readonly FlowView flowView;
		private readonly ToolSetController toolSetController;
		private readonly Dictionary<Tool, IToolWrapper> toolSet = new Dictionary<Tool, IToolWrapper>();

		public IToolWrapper CurrentActiveToolWrapper { get; private set; }
		public Tool CurrentActiveTool => toolSetController.SelectedTool;

		public ToolsManager(ToolSetController toolSetController, FlowView flowView)
		{
			this.flowView = flowView;
			this.toolSetController = toolSetController;
			toolSetController.ToolSelected += ToolSetController_ToolSelected;
			toolSetController.Project.Repository.ShapesInserted += Repository_ShapesInserted;
			toolSetController.Project.Repository.ShapesUpdated += Repository_ShapesUpdated;
			toolSetController.Project.Repository.ShapesDeleted += Repository_ShapesDeleted;
		}

		private void Repository_ShapesDeleted(object sender, RepositoryShapesEventArgs e)
		{
			if (!flowView.DiagramEdited) {
				CurrentActiveToolWrapper?.ShapesDeleted(e.Shapes.ToList());
			}
		}

		private void Repository_ShapesUpdated(object sender, RepositoryShapesEventArgs e)
		{
			if (!flowView.DiagramEdited) {
				CurrentActiveToolWrapper?.ShapesUpdated(e.Shapes.ToList());
			}
		}

		private void Repository_ShapesInserted(object sender, RepositoryShapesEventArgs e)
		{
			if (!flowView.DiagramEdited) {
				CurrentActiveToolWrapper?.ShapesInserted(e.Shapes.ToList());
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
			CurrentActiveToolWrapper = toolSet[tool];
		}
	}
}
