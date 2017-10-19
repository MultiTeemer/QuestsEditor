using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.CodeEditing.SyntaxRewriters
{
	class ClassConstructorCallRemover : SyntaxRewriter
	{
		private readonly ISymbol typeToRemove;

		public ClassConstructorCallRemover(ISymbol typeToRemove, Code code)
			: base(code)
		{
			this.typeToRemove = typeToRemove;
		}

		public override IReadOnlyList<DocumentId> GetDocumentsIdsToModify()
		{
			return GetIdsOfDocsWithReferencesToSymbol(typeToRemove);
		}

		public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
		{
			var model = Compilation.GetSemanticModel(node.SyntaxTree);
			var expr = node.Expression;
			if (expr is ObjectCreationExpressionSyntax oces) {
				if (ReferenceEquals(model.GetTypeInfo(expr).Type, typeToRemove)) {
					return null;
				}
			}

			return base.VisitExpressionStatement(node);
		}

		public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			if (node.Declaration.Variables.Count == 1) {
				var model = Compilation.GetSemanticModel(node.SyntaxTree);
				var type = model.GetTypeInfo(node.Declaration.Variables.First().Initializer.Value).Type;
				if (ReferenceEquals(type, typeToRemove)) {
					return null;
				}
			}

			return base.VisitLocalDeclarationStatement(node);
		}
	}
}
