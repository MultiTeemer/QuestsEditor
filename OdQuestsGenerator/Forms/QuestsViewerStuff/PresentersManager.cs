using Dataweb.NShape;
using OdQuestsGenerator.CodeReaders;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Presenters;
using OdQuestsGenerator.Utils;

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

			var initData = node.Quest.Data.FirstOfTypeOrDefault<InitializationData>();
			if (initData == null || initData.InitializationPlaces.Count == 0) {
				fillStyle = notActiveQuestFillStyle;
			}

			var notEditableLinks = node.Quest.Data.FirstOfTypeOrDefault<NotEditableLinks>();
			if (notEditableLinks != null) {
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
