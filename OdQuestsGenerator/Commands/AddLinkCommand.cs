using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.CodeEditing.SyntaxRewriters;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Commands
{
	class AddLinkCommand : Command
	{
		private readonly Link link;

		private CodeSnapshot snapshot;

		public AddLinkCommand(Link link, EditingContext context)
			: base(context)
		{
			this.link = link;
		}

		public override void Do()
		{
			var sym2 = Context.CodeEditor.GetQuestClassSymbol(link.Node2.Quest);
			var rewriter = new ComponentIsFinishedCallAdder(
				link.Node1.Quest,
				sym2,
				Context.Code
			);
			snapshot = Context.CodeEditor.ApplySyntaxRewriters(rewriter);

			Context.Flow.Graph.AddLink(link);

			CodeFixes.FixQuestInitializationOrder(link.Node2.Quest, Context);
		}

		public override void Undo()
		{
			Context.Flow.Graph.RemoveLink(link);

			Context.CodeEditor.ApplySnapshot(snapshot);
		}
	}
}
