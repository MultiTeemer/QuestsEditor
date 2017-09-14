using OdQuestsGenerator.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Utils;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OdQuestsGenerator.DataTransformers
{
	static class FromCodeTransformer
	{
		public static Quest ReadQuest(SyntaxTree data)
		{
			var @namespace = FetchNameSpaceDecl(data);
			var states = FetchStates(@namespace);

			return new Quest {
				SectorName = FetchSectorName(@namespace),
				Name = FetchQuestName(@namespace),
				States = states,
				FinalState = FetchFinalState(@namespace, states),
			};
		}

		public static string FetchSectorName(SyntaxTree data)
		{
			return FetchSectorName(FetchNameSpaceDecl(data));
		}

		public static List<Tuple<string, string>> FetchQuestToQuestLink(SyntaxTree data)
		{
			var res = new List<Tuple<string, string>>();
			var @namespace = FetchNameSpaceDecl(data);
			var classDecl = @namespace.GetFirstOfType<ClassDeclarationSyntax>();
			var initMethodDecl = classDecl.OfType<MethodDeclarationSyntax>().FirstOrDefault(d => d.Identifier.ToString() == "Initialize");
			var body = initMethodDecl.Body;

			var questsCtrsCalls = GetAllQuestsInitializers(body.Statements);
			var questsIdentifiers = FetchQuestsVariables(body.Statements);

			Func<string, string> trimQuestKeyword = str => str.Remove(str.Length - "Quest".Length);

			foreach (var call in questsCtrsCalls) {
				var initList = call.Initializer;
				if (initList != null) {
					var expression = initList.Expressions
						.OfType<AssignmentExpressionSyntax>()
						.FirstOrDefault(expr => expr.Left.As<IdentifierNameSyntax>().Identifier.ValueText == "ReachedCondition")
						?.Right
					;
					if (expression != null && AccessToComponentIsFinishedProperty(expression)) {
						var parentQuestIdentifierName = expression
							.As<MemberAccessExpressionSyntax>().Expression
							.As<MemberAccessExpressionSyntax>().Expression.As<IdentifierNameSyntax>()
							.Identifier;
						var parentQuestType = questsIdentifiers[parentQuestIdentifierName.ValueText];
						res.Add(Tuple.Create(trimQuestKeyword(parentQuestType.ToString()), trimQuestKeyword(call.Type.ToString())));
					}
				}
			}
		
			return res;
		}

		private static List<ObjectCreationExpressionSyntax> GetAllQuestsInitializers(SyntaxList<StatementSyntax> statements)
		{
			var withoutVariable = statements
				.OfType<ExpressionStatementSyntax>()
				.Select(expr => expr.Expression)
				.OfType<ObjectCreationExpressionSyntax>()
			;
			var withVariables = statements
				.OfType<LocalDeclarationStatementSyntax>()
				.Where(
					decl =>
						decl.Declaration.Variables.Count == 1
						&& decl.Declaration.Variables[0].Initializer.Value is ObjectCreationExpressionSyntax
				)
				.Select(decl => decl.Declaration.Variables[0].Initializer.Value as ObjectCreationExpressionSyntax)
			;

			return withoutVariable.Concat(withVariables).ToList();
		}

		private static Dictionary<string, string> FetchQuestsVariables(SyntaxList<StatementSyntax> statements)
		{
			return statements
				.OfType<LocalDeclarationStatementSyntax>()
				.Where(
					decl =>
						decl.Declaration.Variables.Count == 1
						&& decl.Declaration.Variables[0].Initializer.Value is ObjectCreationExpressionSyntax
				)
				.ToDictionary(
					decl => decl.Declaration.Variables[0].Identifier.ValueText,
					decl => (decl.Declaration.Variables[0].Initializer.Value as ObjectCreationExpressionSyntax).Type.ToString()
				)
			;
		}

		private static bool AccessToComponentIsFinishedProperty(ExpressionSyntax expr)
		{
			return expr is MemberAccessExpressionSyntax
				&& expr.As<MemberAccessExpressionSyntax>().Name.Identifier.ValueText == "IsFinished"
				&& expr.As<MemberAccessExpressionSyntax>().Expression.As<MemberAccessExpressionSyntax>()?.Name.Identifier.ValueText == "Component";
		}

		private static NamespaceDeclarationSyntax FetchNameSpaceDecl(SyntaxTree data)
		{
			return data.GetRoot().GetFirstOfType<NamespaceDeclarationSyntax>();
		}

		private static string FetchSectorName(NamespaceDeclarationSyntax @namespace)
		{
			return (@namespace.Name as QualifiedNameSyntax)?.Right?.ToString();
		}

		private static string FetchQuestName(NamespaceDeclarationSyntax @namespace)
		{
			var classDecl = @namespace.GetFirstOfType<ClassDeclarationSyntax>();

			return classDecl.Identifier.ToString();
		}

		private static List<State> FetchStates(NamespaceDeclarationSyntax @namespace)
		{
			var enumDecl = @namespace.GetFirstOfType<EnumDeclarationSyntax>();
			var enumMembersDecl = enumDecl.ChildNodes().OfType<EnumMemberDeclarationSyntax>().ToArray();

			return enumMembersDecl.Select(mDecl => new State { Name = mDecl.Identifier.ToString() }).ToList();
		}

		private static State FetchFinalState(NamespaceDeclarationSyntax @namespace, List<State> states)
		{
			var componentDecl = @namespace.GetFirstOfType<ClassDeclarationSyntax>();
			var firstGenericBaseTypeDecl = componentDecl.BaseList.Types.FirstOrDefault(t => t.Type is GenericNameSyntax).Type as GenericNameSyntax;
			if (firstGenericBaseTypeDecl.Identifier.ToString() == "SectorComponent")
				return null;

			var constructorDecl = componentDecl.GetFirstOfType<ConstructorDeclarationSyntax>();
			var enumAccessExpression = constructorDecl.Initializer.ArgumentList.Arguments[0].Expression as MemberAccessExpressionSyntax;
			return states.FirstOrDefault(s => s.Name == enumAccessExpression?.Name?.ToString());
		}
	}
}
