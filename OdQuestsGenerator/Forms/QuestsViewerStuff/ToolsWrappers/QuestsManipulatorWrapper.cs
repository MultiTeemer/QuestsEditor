using System.Collections.Generic;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class QuestsManipulatorWrapper : ToolWrapper
	{
		public QuestsManipulatorWrapper(EditingContext context)
			: base(context)
		{}

		public override void ShapesUpdated(List<Shape> affectedShapes)
		{
			base.ShapesUpdated(affectedShapes);

			if (affectedShapes.Count > 0) {
				var shape = affectedShapes.First() as CaptionedShapeBase;
				var quest = Context.View.GetNodeForShape(shape).Quest;
				Context.History.Do(new RenameQuestCommand(quest, shape.Text, Context));
			}
		}
	}
}
