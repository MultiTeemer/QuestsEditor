using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.CodeEditing.SyntaxRewriters;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Commands
{
	class RemoveLinkCommand : Command
	{
		private readonly Link link;

		private CodeSnapshot snapshot;

		public RemoveLinkCommand(Link link, EditingContext context)
			: base(context)
		{
			this.link = link;
		}

		public override void Do()
		{
			var sym1 = GetQuestClassSymbol(link.Node1.Quest);
			var sym2 = GetQuestClassSymbol(link.Node2.Quest);
			var rewriter = new ComponentIsFinishedCallInInitializerRemover(
				sym2,
				sym1,
				Context.Code
			);
			snapshot = Context.CodeEditor.ApplySyntaxRewriters(rewriter);

			Context.Flow.Graph.RemoveLink(link);
		}

		public override void Undo()
		{
			Context.CodeEditor.ApplySnapshot(snapshot);

			Context.Flow.Graph.AddLink(link);
		}

		private ISymbol GetQuestClassSymbol(Quest quest)
		{
			var cb = Context.Code.QuestsAndCodeBulks[quest];
			var decl = cb.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

			return Context.CodeEditor.GetSymbolFor(decl, cb);
		}
	}
}
