using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.CodeEditing.SyntaxRewriters;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

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
			var sym1 = GetQuestClassSymbol(link.Node1.Quest);
			var sym2 = GetQuestClassSymbol(link.Node2.Quest);
			var rewriter = new ComponentIsFinishedCallAdder(
				link.Node1.Quest,
				link.Node2.Quest,
				sym1,
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

		private ISymbol GetQuestClassSymbol(Quest quest)
		{
			var cb = Context.Code.QuestsAndCodeBulks[quest];
			var decl = cb.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

			return Context.CodeEditor.GetSymbolFor(decl, cb);
		}
	}
}
