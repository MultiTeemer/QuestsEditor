using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.SyntaxRewriters
{
	class ComponentIsFinishedCallAdder : SyntaxRewriter
	{
		private readonly Quest quest1;
		private readonly Quest quest2;

		private readonly ISymbol type1;
		private readonly ISymbol type2;

		public ComponentIsFinishedCallAdder(Quest quest1, Quest quest2, ISymbol type1, ISymbol type2, Code code)
			: base(code)
		{
			this.quest1 = quest1;
			this.quest2 = quest2;
			this.type1 = type1;
			this.type2 = type2;
		}

		public override IReadOnlyList<DocumentId> GetDocumentsIdsToModify() => GetIdsOfDocsWithReferencesToSymbol(type2);

		public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			var model = Compilation.GetSemanticModel(node.SyntaxTree);
			var callableType = model.GetTypeInfo(node).Type;
			if (callableType == type2) {
				var linkExpr = node.Initializer.FindLinkExpression();
				var newLinkText = $"{ CodeEditor.FormatQuestNameForVar(quest1.Name) }.Component.IsFinished";
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
