using System;
using System.Collections.Generic;
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
			}

			if (node is BinaryExpressionSyntax bes) {
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
			}

			if (node is BinaryExpressionSyntax bes) {
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

		public static MethodDeclarationSyntax GetSectorInitializationFunction(this SyntaxTree tree) =>
			tree.GetRoot().DescendantNodes()
			.OfType<MethodDeclarationSyntax>()
			.FirstOrDefault(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.MethodDeclaration) && m.Identifier.ValueText == "Initialize");

		public static ClassDeclarationSyntax GetQuestClass(this SyntaxTree tree) =>
			tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().LastOrDefault();

		public static List<Tuple<string, MethodDeclarationSyntax>> GetAllStatesMethods(this SyntaxTree tree)
		{
			var classDecl = tree.GetQuestClass();
			if (classDecl == null) {
				throw new Exception("Couldn't find quest class in syntax tree");
			}

			var stateMethods = classDecl.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.Where(m => m.AttributeLists.Count > 0 && m.AttributeLists.Any(al => al.Attributes.Any(attr => attr.Name.ToString() == "State")))
				.ToList();

			var res = new List<Tuple<string, MethodDeclarationSyntax>>();

			foreach (var sm in stateMethods) {
				var attr = sm.DescendantNodes().OfType<AttributeSyntax>().First(a => a.Name.ToString() == "State");
				res.Add(Tuple.Create(attr.GetQuestStateName(), sm));
			}

			return res;
		}

		public static string GetQuestStateName(this AttributeSyntax attributeSyntax)
		{
			return attributeSyntax.ArgumentList.Arguments[0].Expression.As<MemberAccessExpressionSyntax>()?.Name?.ToString();
		}
	}
}
