using System.Collections.Generic;
using Dataweb.NShape;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class StubWrapper : ToolWrapper
	{
		public StubWrapper(EditingContext context)
			: base(context)
		{}

		public override void ShapesInserted(List<Shape> affectedShapes)
		{
			base.ShapesInserted(affectedShapes);
		}

		public override void ShapesUpdated(List<Shape> affectedShapes)
		{
			base.ShapesUpdated(affectedShapes);
		}

		public override void ShapesDeleted(List<Shape> affectedShapes)
		{
			base.ShapesDeleted(affectedShapes);
		}
	}
}
