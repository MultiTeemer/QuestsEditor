using System.Collections.Generic;
using Dataweb.NShape;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class StubWrapper : ToolWrapper<Tool>
	{
		public StubWrapper(EditingContext context, Tool tool)
			: base(context, tool)
		{}

		public override void OnShapesInserted(List<Shape> affectedShapes)
		{
			base.OnShapesInserted(affectedShapes);
		}

		public override void OnShapesUpdated(List<Shape> affectedShapes)
		{
			base.OnShapesUpdated(affectedShapes);
		}

		public override void OnShapesDeleted(List<Shape> affectedShapes)
		{
			base.OnShapesDeleted(affectedShapes);
		}
	}
}
