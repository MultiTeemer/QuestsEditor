using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.CodeEditing.SyntaxRewriters
{
	class ComponentIsFinishedCallInInitializerRemover : SyntaxRewriter
	{
		private readonly ISymbol typeOfExpression;
		private readonly ISymbol typeToRemove;

		public ComponentIsFinishedCallInInitializerRemover(ISymbol typeOfExpression, ISymbol typeToRemove, Code code)
			: base(code)
		{
			this.typeOfExpression = typeOfExpression;
			this.typeToRemove = typeToRemove;
		}

		public override IReadOnlyList<DocumentId> GetDocumentsIdsToModify() => GetIdsOfDocsWithReferencesToSymbol(typeOfExpression);

		public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			var model = Compilation.GetSemanticModel(node.SyntaxTree);
			var callableType = model.GetTypeInfo(node).Type;
			if (ReferenceEquals(callableType, typeOfExpression)) {
				var linkExpr = node.Initializer.FindLinkExpression();
				if (linkExpr != null && linkExpr.Right.IsEditableInterQuestLinkExpression()) {
					var linksCount = linkExpr.Right.CountOfLinks();
					if (linksCount == 1) {
						return node.WithInitializer(node.Initializer.RemoveNode(linkExpr, SyntaxRemoveOptions.KeepNoTrivia));
					}

					var txt = linkExpr.Right.ToString();
					var linkTxt = $"{CodeEditor.FormatQuestClassNameForVar(typeToRemove.Name)}.Component.IsFinished";
					var newTxt = Regex.Replace(Regex.Replace(txt, $"&&\\s+{linkTxt}\\s?", ""), $"\\s?{linkTxt}\\s+&&", "");
					var newLinkExpr = linkExpr.WithRight(SyntaxFactory.ParseExpression(newTxt));
					var newInitializer = node.Initializer.ReplaceNode(linkExpr, newLinkExpr);

					return node.WithInitializer(newInitializer);
				}

				return node;
			}

			return base.VisitObjectCreationExpression(node);
		}
	}
}
