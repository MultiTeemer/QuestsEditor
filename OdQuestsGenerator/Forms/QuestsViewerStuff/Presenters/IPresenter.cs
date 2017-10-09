using System;
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
		FillStyle FillStyle { get; }
		LineStyle StrokeStyle { get; }
	}

	interface ILinkPresenter : IPresenter
	{
		LineStyle Style { get; }
	}

	abstract class NodePresenter : INodePresenter
	{
		protected readonly Project Project;

		public FillStyle FillStyle { get; set; }
		public LineStyle StrokeStyle { get; set; }

		public void Apply(Shape shape)
		{
			if (!(shape is IPlanarShape)) {
				throw new System.ArgumentException("Wrong type for argument", nameof(shape));
			}

			shape.LineStyle = StrokeStyle;
			shape.As<IPlanarShape>().FillStyle = FillStyle;
		}

		protected NodePresenter(FillStyle fillStyle, LineStyle strokeStyle, Project project)
		{
			Project = project;
			FillStyle = fillStyle;
			StrokeStyle = strokeStyle;

			FillStyle.Name = Guid.NewGuid().ToString();
			StrokeStyle.Name = Guid.NewGuid().ToString();
			if (!Project.Design.FillStyles.Contains(fillStyle)) {
				Project.Design.FillStyles.Add(FillStyle, FillStyle);
			}
			if (!project.Design.LineStyles.Contains(strokeStyle)) {
				Project.Design.LineStyles.Add(strokeStyle, strokeStyle);
			}
		}
	}

	abstract class LinkPresenter : ILinkPresenter
	{
		protected readonly Project Project;

		public LineStyle Style { get; set; }

		public void Apply(Shape shape)
		{
			shape.LineStyle = Style;
		}

		protected LinkPresenter(LineStyle style, Project project)
		{
			Project = project;
			Style = style;

			Style.Name = Guid.NewGuid().ToString();
			if (!project.Design.LineStyles.Contains(style)) {
				project.Design.LineStyles.Add(style, style);
			}
		}
	}
}
