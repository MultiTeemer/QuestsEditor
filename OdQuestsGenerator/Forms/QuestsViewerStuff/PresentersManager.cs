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

		public PresentersManager(Project project)
		{
			this.project = project;
		}

		public IPresenter GetPresenterFor(Node node)
		{
			var initData = node.Quest.Data.FirstOfTypeOrDefault<InitializationData>();
			var fillStyle = initData == null || initData.InitializationPlaces.Count == 0 ? notActiveQuestFillStyle : null;

			return new DefaultQuestPresenter(project, fillStyle);
		}

		public IPresenter GetPresenterFor(Link link)
		{
			return new DefaultLinkPresenter(project);
		}
	}
}
