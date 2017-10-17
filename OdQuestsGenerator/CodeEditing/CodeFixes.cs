using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeReaders.SyntaxVisitors;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.CodeEditing
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

				if (Result == null) {
					var model = Code.Compilation.GetSemanticModel(node.SyntaxTree);
					if (model.GetTypeInfo(node).Type == type) {
						Result = node.GetParentOfType<StatementSyntax>();
					}
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
			foreach (var linkedQuest in linkedQuests) {
				var finder = new InitializationStatementsFinder(
					context.Code,
					context.CodeEditor.GetQuestClassSymbol(linkedQuest)
				);
				finder.Visit(block);
				var initStatement = finder.Result;
				var pos = block.Statements.IndexOf(initStatement);
				if (pos > lastStatementPos) {
					lastStatement = initStatement;
					lastStatementPos = pos;
				}
			}

			var replaceeFinder = new InitializationStatementsFinder(
				context.Code,
				context.CodeEditor.GetQuestClassSymbol(quest)
			);
			replaceeFinder.Visit(block);
			var replacee = replaceeFinder.Result;
			var idx = block.Statements.IndexOf(replacee);

			if (idx < lastStatementPos) {
				var newStatements = block.Statements.RemoveAt(idx).Insert(lastStatementPos, replacee);
				cb.Tree = cb.Tree.GetRoot().ReplaceNode(
					cb.Tree.GetSectorInitializationFunction(),
					init.WithBody(block.WithStatements(newStatements))
				).SyntaxTree;
			}
		}
	}
}
