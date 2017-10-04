using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OdQuestsGenerator.Utils
{
	public static class CodeAnalysisExtensions
	{
		public static bool IsEditableInterQuestLinkExpression(this ExpressionSyntax node)
		{
			if (node is MemberAccessExpressionSyntax) {
				return node.AccessToComponentIsFinishedProperty();
			} else if (node is BinaryExpressionSyntax) {
				var bes = node as BinaryExpressionSyntax;
				return bes.OperatorToken.ToString() == "&&"
					&& bes.Left.IsEditableInterQuestLinkExpression()
					&& bes.Right.IsEditableInterQuestLinkExpression();
			}

			return false;
		}

		public static int CountOfLinks(this ExpressionSyntax node)
		{
			if (node is MemberAccessExpressionSyntax) {
				return 1;
			} else if (node is BinaryExpressionSyntax) {
				var bes = node as BinaryExpressionSyntax;
				return bes.Left.CountOfLinks() + bes.Right.CountOfLinks();
			}

			return 0;
		}

		public static AssignmentExpressionSyntax FindLinkExpression(this InitializerExpressionSyntax initializer)
			=> initializer.Expressions
				.OfType<AssignmentExpressionSyntax>()
				.FirstOrDefault(aes => aes.Left.ToString() == "ReachedCondition");

		public static bool AccessToComponentIsFinishedProperty(this ExpressionSyntax expr)
			=> expr is MemberAccessExpressionSyntax
				&& expr.As<MemberAccessExpressionSyntax>().Name.Identifier.ValueText == "IsFinished"
				&& expr.As<MemberAccessExpressionSyntax>().Expression.As<MemberAccessExpressionSyntax>()?.Name.Identifier.ValueText == "Component";

		public static MethodDeclarationSyntax GetSectorInitializationFunction(this SyntaxTree tree)
		{
			return tree.GetRoot().DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.FirstOrDefault(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.MethodDeclaration) && m.Identifier.ValueText == "Initialize");
		}
	}
}
