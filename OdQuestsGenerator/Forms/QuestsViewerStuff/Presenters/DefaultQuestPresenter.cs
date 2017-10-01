using Dataweb.NShape;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Presenters
{
	class DefaultQuestPresenter : NodePresenter
	{
		private static readonly FillStyle defaultFillStyle = new FillStyle(
			"",
			new ColorStyle("", System.Drawing.Color.LightBlue),
			new ColorStyle("", System.Drawing.Color.White)
		);
		private static readonly LineStyle defaultStrokeStyle = new LineStyle(
			"",
			2,
			new ColorStyle("", System.Drawing.Color.Black)
		);

		public DefaultQuestPresenter(IFillStyle fillStyle = null, ILineStyle strokeStyle = null)
			: base(fillStyle ?? defaultFillStyle, strokeStyle ?? defaultStrokeStyle)
		{}
	}
}
