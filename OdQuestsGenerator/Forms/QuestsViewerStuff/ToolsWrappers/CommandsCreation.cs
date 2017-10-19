using Dataweb.NShape;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	static class CommandsCreation
	{
		public static ActivateQuestCommand ActivateQuest(Quest quest, EditingContext context, FlowView flowView) =>
			new ActivateQuestCommand(quest, context.Flow.GetSectorForQuest(quest), context) {
				Done = (_) => flowView.Update(),
				Undone = flowView.Update,
			};

		public static AddQuestCommand AddQuest(Quest quest, Sector sector, EditingContext context, FlowView flowView, Shape shape) =>
			new AddQuestCommand(quest, sector, context) {
				Done = (firstTime) => {
					var n = context.Flow.Graph.FindNodeForQuest(quest);
					if (firstTime) {
						flowView.RegisterShapeForNode(n, shape);
					} else {
						flowView.AddShapeForNode(n, shape);
					}
				},
				Undone = () => {
					flowView.RemoveNodeShape(shape);
				},
			};

		public static RemoveQuestCommand RemoveQuest(Quest quest, EditingContext context, Node node, FlowView flowView, Shape shape) =>
			new RemoveQuestCommand(quest, context) {
				Done = (firstTime) => {
					if (!firstTime) {
						flowView.RemoveNodeShape(shape);
					}
				},
				Undone = () => flowView.AddShapeForNode(node, shape),
			};

		public static AddLinkCommand AddLink(Link link, EditingContext context, FlowView flowView) =>
			new AddLinkCommand(link, context) {
				Done = (_) => flowView.AddShapeLink(link),
				Undone = () => flowView.RemoveShapeLink(link),
			};

		public static RemoveLinkCommand RemoveLink(Link link, EditingContext context, FlowView flowView) =>
			new RemoveLinkCommand(link, context) {
				Done = (_) => flowView.RemoveShapeLink(link),
				Undone = () => flowView.AddShapeLink(link),
			};

		public static RenameQuestCommand RenameQuest(Quest quest, string oldName, string newName, EditingContext context, FlowView flowView, Box shape) =>
			new RenameQuestCommand(quest, oldName, newName, context) {
				Done = (_) => SetCaption(shape, newName),
				Undone = () => SetCaption(shape, oldName),
			};

		private static void SetCaption(Box box, string newCaption) => box.SetCaptionText(0, newCaption);
	}
}
