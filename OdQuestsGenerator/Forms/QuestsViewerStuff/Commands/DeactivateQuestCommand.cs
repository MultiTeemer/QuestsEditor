using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Commands
{
	class DeactivateQuestCommand : Command
	{
		public DeactivateQuestCommand(Quest quest, EditingContext context)
			: base(context)
		{
		}

		public override void Do()
		{
		}

		public override void Undo()
		{
		}
	}
}
