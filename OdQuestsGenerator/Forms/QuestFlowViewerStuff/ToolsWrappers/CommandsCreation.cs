using Dataweb.NShape;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Forms.QuestFlowViewerStuff.ToolsWrappers
{
	static class CommandsCreation
	{
		public static RemoveQuestActionCommand RemoveQuestAction(QuestAction action, Quest quest, EditingContext context, QuestFlowView flowView, Shape shape) =>
			new RemoveQuestActionCommand(action, quest, context) {
				Done = (firstTime) => {
					flowView.RemoveActionShape(shape);
					flowView.ArrangeShapes();
				},
				Undone = () => {
					flowView.AddActionShape(action, shape);
					flowView.ArrangeShapes();
				},
			};
	}
}
