using System.Collections.Generic;
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
			var sector = context.Flow.GetSectorForQuest(quest);

			var questsToProcess = new Queue<Quest>();
			questsToProcess.Enqueue(quest);

			var processedQuests = new HashSet<Quest>();

			while (questsToProcess.Any()) {
				var curQuest = questsToProcess.Dequeue();
				processedQuests.Add(curQuest);
				var cb = context.Code.SectorsAndCodeBulks[context.Flow.GetSectorForQuest(curQuest)];
				var init = context.Code.GetMappedCode(cb).GetSyntaxTreeAsync().Result.GetSectorInitializationFunction();
				var block = init.Body;
				var linkedQuests = sector.Quests.Where(q => context.Flow.Graph.ExistsLink(q, curQuest));

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
						lastStatementPos = pos;
					}
				}

				var replaceeFinder = new InitializationStatementsFinder(
					context.Code,
					context.CodeEditor.GetQuestClassSymbol(curQuest)
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

					var relatedQuests = sector.Quests.Where(q => context.Flow.Graph.ExistsLink(curQuest, q));
					foreach (var relatedQuest in relatedQuests) {
						if (!processedQuests.Contains(relatedQuest)) {
							questsToProcess.Enqueue(relatedQuest);
						}
					}
				}
			}
		}
	}
}
