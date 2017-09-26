using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.SyntaxRewriters
{
	class ComponentIsFinishedCallInInitializerRemover : SyntaxRewriter
	{
		private readonly ISymbol typeOfExpression;
		private readonly ISymbol typeToRemove;

		public ComponentIsFinishedCallInInitializerRemover(ISymbol typeOfExpression, ISymbol typeToRemove, Solution solution, Compilation compilation)
			: base(solution, compilation)
		{
			this.typeOfExpression = typeOfExpression;
			this.typeToRemove = typeToRemove;
		}

		public override IReadOnlyList<DocumentId> GetDocumentsIdsToModify()
		{
			return GetIdsOfDocsWithReferencesToSymbol(typeToRemove)
				.Concat(GetIdsOfDocsWithReferencesToSymbol(typeOfExpression))
				.ToList();
		}

		public override SyntaxNode VisitInitializerExpression(InitializerExpressionSyntax node)
		{
			var isReachedExpr = node.Expressions.OfType<AssignmentExpressionSyntax>().FirstOrDefault(e => e.Left.ToString() == "ReachedCondition");
			var maes = isReachedExpr?.Right as MemberAccessExpressionSyntax;
			var parent = node.Parent as ObjectCreationExpressionSyntax;
			if (parent != null && maes != null && maes.Name.ToString() == "IsFinished") {
				var model = Compilation.GetSemanticModel(node.SyntaxTree);
				var t1 = model.GetTypeInfo(parent).Type;
				var t2 = model.GetTypeInfo((maes.Expression as MemberAccessExpressionSyntax)?.Expression).Type;
				if (t1 == typeOfExpression && t2 == typeToRemove) {
					return node.RemoveNode(isReachedExpr, SyntaxRemoveOptions.AddElasticMarker);
				}
			}

			return base.VisitInitializerExpression(node);
		}
	}
}
