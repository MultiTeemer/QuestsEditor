using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Commands
{
	class RemoveQuestCommand : Command
	{
		private readonly Quest quest;
		private readonly Node node;
		private readonly CodeBulk codeBulk;

		public RemoveQuestCommand(Quest questToDelete, EditingContext context)
			: base(context)
		{
			quest = questToDelete;
			node = Context.Flow.Graph.FindNodeForQuest(quest);
			codeBulk = Context.Code.QuestsAndCodeBulks[quest];
		}

		public override void Do()
		{
			Context.Flow.Graph.RemoveNode(node);

			Context.Code.Remove(codeBulk);
		}

		public override void Undo()
		{
			Context.Code.Add(codeBulk);

			Context.Code.QuestsAndCodeBulks[quest] = codeBulk;

			Context.Flow.Graph.AddNode(node);
		}
	}
}
