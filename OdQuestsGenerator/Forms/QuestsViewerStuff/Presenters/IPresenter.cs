using Dataweb.NShape;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Presenters
{
	interface IPresenter
	{
		void Apply(Shape shape);
	}

	interface INodePresenter : IPresenter
	{
		IFillStyle FillStyle { get; }
		ILineStyle StrokeStyle { get; }
	}

	interface ILinkPresenter : IPresenter
	{
		ILineStyle Style { get; }
	}

	abstract class NodePresenter : INodePresenter
	{
		public IFillStyle FillStyle { get; set; }
		public ILineStyle StrokeStyle { get; set; }

		public void Apply(Shape shape)
		{
			if (!(shape is IPlanarShape)) {
				throw new System.ArgumentException("Wrong type for argument", nameof(shape));
			}

			shape.LineStyle = StrokeStyle;
			shape.As<IPlanarShape>().FillStyle = FillStyle;
		}

		protected NodePresenter(IFillStyle fillStyle, ILineStyle strokeStyle)
		{
			FillStyle = fillStyle;
			StrokeStyle = strokeStyle;
		}
	}

	abstract class LinkPresenter : ILinkPresenter
	{
		public ILineStyle Style { get; set; }

		public void Apply(Shape shape)
		{
			shape.LineStyle = Style;
		}

		protected LinkPresenter(ILineStyle style)
		{
			Style = style;
		}
	}
}
