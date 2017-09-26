using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.SyntaxRewriters
{
	class ClassConstructorCallRemover : SyntaxRewriter
	{
		private readonly Compilation compilation;
		private readonly ISymbol typeToRemove;

		public ClassConstructorCallRemover(ISymbol typeToRemove, Solution solution, Compilation compilation)
			: base(solution, compilation)
		{
			this.compilation = compilation;
			this.typeToRemove = typeToRemove;
		}

		public override IReadOnlyList<DocumentId> GetDocumentsIdsToModify()
		{
			return GetIdsOfDocsWithReferencesToSymbol(typeToRemove);
		}

		public override SyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
		{
			var model = compilation.GetSemanticModel(node.SyntaxTree);
			var expr = node.Expression;
			var oces = expr as ObjectCreationExpressionSyntax;
			if (oces != null) {
				if (model.GetTypeInfo(expr).Type == typeToRemove) {
					return null;
				}
			}

			return base.VisitExpressionStatement(node);
		}

		public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			if (node.Declaration.Variables.Count == 1) {
				var model = compilation.GetSemanticModel(node.SyntaxTree);
				var type = model.GetTypeInfo(node.Declaration.Variables.First().Initializer.Value).Type;
				if (type == typeToRemove) {
					return null;
				}
			}

			return base.VisitLocalDeclarationStatement(node);
		}
	}
}
