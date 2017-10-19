using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.CodeEditing.SyntaxRewriters
{
	class ComponentIsFinishedCallAdder : SyntaxRewriter
	{
		private readonly Quest quest;
		private readonly ISymbol typeOfNextQuest;

		public ComponentIsFinishedCallAdder(Quest quest, ISymbol typeOfNextQuest, Code code)
			: base(code)
		{
			this.quest = quest;
			this.typeOfNextQuest = typeOfNextQuest;
		}

		public override IReadOnlyList<DocumentId> GetDocumentsIdsToModify() => GetIdsOfDocsWithReferencesToSymbol(typeOfNextQuest);

		public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			var model = Compilation.GetSemanticModel(node.SyntaxTree);
			var callableType = model.GetTypeInfo(node).Type;
			if (ReferenceEquals(callableType, typeOfNextQuest)) {
				var linkExpr = node.Initializer.FindLinkExpression();
				var newLinkText = $"{ CodeEditor.FormatQuestNameForVar(quest.Name) }.Component.IsFinished";
				var newInitializer = node.Initializer;
				if (linkExpr == null) {
					var linkExprText = $"ReachedCondition = {newLinkText}";
					newInitializer = node.Initializer.AddExpressions(SyntaxFactory.ParseExpression(linkExprText));
				} else if (linkExpr.Right.IsEditableInterQuestLinkExpression()) {
					var newLinkExpr = linkExpr.WithRight(SyntaxFactory.ParseExpression($"{linkExpr.Right.ToString().Erase("\\,\\s+")} && {newLinkText},\n"));
					newInitializer = node.Initializer.ReplaceNode(linkExpr, newLinkExpr);
				}

				return node.WithInitializer(newInitializer);
			}

			return base.VisitObjectCreationExpression(node);
		}
	}
}
