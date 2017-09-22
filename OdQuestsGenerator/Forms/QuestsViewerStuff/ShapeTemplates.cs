using Dataweb.NShape;
using Dataweb.NShape.GeneralShapes;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
{
	class ShapeTemplatesFactory
	{
		private readonly Project project;

		public ShapeTemplatesFactory(Project project)
		{
			this.project = project;
		}

		public Polyline GetLinkTemplate()
		{
			var arrow = (Polyline)project.ShapeTypes["Polyline"].CreateInstance();
			arrow.EndCapStyle = project.Design.CapStyles.ClosedArrow;

			return arrow;
		}

		public Box GetQuestTemplate(string name = null)
		{
			var shape = (Box)project.ShapeTypes["Box"].CreateInstance();
			shape.SetCaptionText(0, name ?? "Default");
			shape.Width = FlowView.ShapeSize;
			shape.Height = FlowView.ShapeSize / 2;

			return shape;
		}
	}
}
