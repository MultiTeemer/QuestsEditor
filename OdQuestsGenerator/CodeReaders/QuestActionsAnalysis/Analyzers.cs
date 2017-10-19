using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.CodeReaders.QuestActionsAnalysis
{
	[AttributeUsage(AttributeTargets.Method)]
	class QuestActionAnalyzerAttribute : Attribute {}

	static class Analyzers
	{
		[QuestActionAnalyzer]
		public static QuestAction SkipFrame(StatementSyntax expression)
		{
			if (expression is YieldStatementSyntax) {
				var expr = expression.As<YieldStatementSyntax>().Expression;
				if (expr != null && expr.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.NullLiteralExpression) {
					return new QuestAction {
						Type = QuestActionType.SkipFrame,
						Source = expression.ToString(),
					};
				}
			}

			return null;
		}

		[QuestActionAnalyzer]
		public static QuestAction WaitSomeTime(StatementSyntax expression)
		{
			if (expression is YieldStatementSyntax) {
				var expr = expression.As<YieldStatementSyntax>().Expression.As<LiteralExpressionSyntax>();
				if (expr != null && expr.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.NumericLiteralExpression) {
					return new QuestAction {
						Type = QuestActionType.WaitSomeTime,
						Source = expression.ToString(),
						Properties = new Dictionary<string, object> {
							["time"] = expr.Token.ValueText
						},
					};
				}
			}

			return null;
		}

		[QuestActionAnalyzer]
		public static QuestAction EndState(StatementSyntax expression)
		{
			if (expression is YieldStatementSyntax) {
				var expr = expression.As<YieldStatementSyntax>().Expression;
				if (expr == null) {
					return new QuestAction {
						Type = QuestActionType.EndState,
						Source = expression.ToString(),
					};
				}
			}

			return null;
		}
	}
}
