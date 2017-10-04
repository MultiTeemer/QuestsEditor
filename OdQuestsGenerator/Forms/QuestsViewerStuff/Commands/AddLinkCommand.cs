using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.SyntaxRewriters;
using OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Commands
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

			Context.FlowView.AddShapeLink(link);
		}

		public override void Undo()
		{
			Context.Flow.Graph.RemoveLink(link);

			Context.CodeEditor.ApplySnapshot(snapshot);

			Context.FlowView.RemoveShapeLink(link);
		}

		private ISymbol GetQuestClassSymbol(Quest quest)
		{
			var cb = Context.Code.QuestsAndCodeBulks[quest];
			var decl = cb.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

			return Context.CodeEditor.GetSymbolFor(decl, cb);
		}
	}
}
