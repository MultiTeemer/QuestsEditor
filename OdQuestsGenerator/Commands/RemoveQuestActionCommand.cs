using System.Linq;
using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Commands
{
	class RemoveQuestActionCommand : Command
	{
		private readonly QuestAction action;
		private readonly Quest quest;
		private readonly QuestActionsData data;
		private readonly int idx;

		private CodeSnapshot snapshot;

		public RemoveQuestActionCommand(QuestAction action, Quest quest, EditingContext context)
			: base(context)
		{
			this.action = action;
			this.quest = quest;

			data = quest.States
				.First(s => s.Data.Ensure<QuestActionsData>().Actions.Contains(action))
				.Data
				.Ensure<QuestActionsData>();
			idx = data.Actions.IndexOf(action);
		}

		public override void Do()
		{
			snapshot = Context.CodeEditor.RemoveNode(action.Id, Context.Code.QuestsAndCodeBulks[quest]);

			data.Actions.RemoveAt(idx);
		}

		public override void Undo()
		{
			Context.CodeEditor.ApplySnapshot(snapshot);

			data.Actions.Insert(idx, action);
		}
	}
}
