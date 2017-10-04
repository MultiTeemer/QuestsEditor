using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeReaders.SyntaxVisitors;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
{
	static class CodeFixes
	{
		private class InitializationStatementsFinder : SyntaxVisitor
		{
			public StatementSyntax Result { get; private set; }

			private readonly ISymbol type;

			public InitializationStatementsFinder(Code code, ISymbol type)
				: base(code)
			{
				this.type = type;
			}

			public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
			{
				base.VisitObjectCreationExpression(node);

				if (Result != null) return;

				var model = Code.Compilation.GetSemanticModel(node.SyntaxTree);
				if (model.GetTypeInfo(node).Type == type) {
					Result = node.GetParentOfType<StatementSyntax>();
				}
			}
		}

		public static void FixQuestInitializationOrder(Quest quest, EditingContext context)
		{
			var cb = context.Code.SectorsAndCodeBulks[context.Flow.GetSectorForQuest(quest)];
			var init = context.Code.GetMappedCode(cb).GetSyntaxTreeAsync().Result.GetSectorInitializationFunction();
			var block = init.Body;

			var model = context.Code.Compilation.GetSemanticModel(block.SyntaxTree);
			var sector = context.Flow.GetSectorForQuest(quest);
			var linkedQuests = sector.Quests.Where(q => context.Flow.Graph.ExistsLink(q, quest));

			var lastStatement = block.Statements.First();
			var lastStatementPos = 0;
			foreach (var q in linkedQuests) {
				var finder = new InitializationStatementsFinder(context.Code, GetQuestClassSymbol(q, context));
				finder.Visit(block);
				var s = finder.Result;
				var pos = block.Statements.IndexOf(s);
				if (pos > lastStatementPos) {
					lastStatement = s;
					lastStatementPos = pos;
				}
			}

			var f = new InitializationStatementsFinder(context.Code, GetQuestClassSymbol(quest, context));
			f.Visit(block);
			var replacee = f.Result;
			var idx = block.Statements.IndexOf(replacee);

			if (idx < lastStatementPos) {
				var ss = block.Statements.RemoveAt(idx);
				ss = ss.Insert(lastStatementPos, replacee);

				cb.Tree = cb.Tree.GetRoot().ReplaceNode(cb.Tree.GetSectorInitializationFunction(), init.WithBody(block.WithStatements(ss))).SyntaxTree;
			}
		}

		private static ISymbol GetQuestClassSymbol(Quest quest, EditingContext Context)
		{
			var cb = Context.Code.QuestsAndCodeBulks[quest];
			var decl = cb.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

			return Context.CodeEditor.GetSymbolFor(decl, cb);
		}
	}
}
