using Dataweb.NShape;
using OdQuestsGenerator.CodeReaders;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Presenters;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
{
	class PresentersManager
	{
		private readonly Project project;

		private static readonly FillStyle notActiveQuestFillStyle = new FillStyle(
			"",
			new ColorStyle("", System.Drawing.Color.LightGray),
			new ColorStyle("", System.Drawing.Color.LightGray)
		);

		private static readonly FillStyle notEditableLinksFillStyle = new FillStyle(
			"",
			new ColorStyle("", System.Drawing.Color.Pink),
			new ColorStyle("", System.Drawing.Color.LightCoral)
		);
		private static readonly LineStyle notEditableLinkStrokeStyle = new LineStyle(
			"",
			3,
			new ColorStyle("", System.Drawing.Color.Red)
		);

		public PresentersManager(Project project)
		{
			this.project = project;
		}

		public IPresenter GetPresenterFor(Node node)
		{
			FillStyle fillStyle = null;
			LineStyle strokeStyle = null;

			if (!node.Quest.IsActive()) {
				fillStyle = notActiveQuestFillStyle;
			}

			if (!node.Quest.IsLinksToEditable()) {
				fillStyle = notEditableLinksFillStyle;
				strokeStyle = notEditableLinkStrokeStyle;
			}

			return new DefaultQuestPresenter(project, fillStyle, strokeStyle);
		}

		public IPresenter GetPresenterFor(Link link)
		{
			return new DefaultLinkPresenter(project);
		}
	}
}
