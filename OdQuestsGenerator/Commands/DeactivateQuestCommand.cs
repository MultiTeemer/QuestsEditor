using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Commands
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
