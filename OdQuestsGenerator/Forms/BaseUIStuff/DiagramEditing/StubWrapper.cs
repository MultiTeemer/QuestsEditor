using System.Collections.Generic;
using Dataweb.NShape;
using OdQuestsGenerator.Commands;

namespace OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing
{
	class StubWrapper : ToolWrapper<Tool, DiagramWrapper>
	{
		public StubWrapper(EditingContext context, Tool tool, DiagramWrapper diagramWrapper)
			: base(context, tool, diagramWrapper)
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
