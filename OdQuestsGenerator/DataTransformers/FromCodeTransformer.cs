using OdQuestsGenerator.Data;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Utils;
using System.Collections.Generic;
using System.Linq;

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
