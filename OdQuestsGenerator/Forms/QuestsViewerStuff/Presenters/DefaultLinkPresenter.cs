using Dataweb.NShape;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Presenters
{
	class DefaultLinkPresenter : LinkPresenter
	{
		private static readonly LineStyle defaultStrokeStyle = new LineStyle(
			"",
			2,
			new ColorStyle("", System.Drawing.Color.Black)
		);

		public DefaultLinkPresenter(Project project, LineStyle style = null)
			: base(style ?? defaultStrokeStyle, project)
		{}
	}
}
