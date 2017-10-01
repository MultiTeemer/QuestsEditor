using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeReaders.SyntaxVisitors;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.CodeReaders
{
	class InitializationData : IData
	{
		public readonly List<Sector> InitializationPlaces = new List<Sector>();
	}

	public static class QuestExtensions
	{
		public static bool IsActive(this Quest quest)
		{
			var initData = quest.Data.FirstOfTypeOrDefault<InitializationData>();
			return initData != null && initData.InitializationPlaces.Count > 0;
		}
	}

	class QuestInitializationReader : CodeReader
	{
		private class QuestInitializationFinder : SyntaxVisitor
		{
			private readonly ISymbol questTypeToFind;

			public List<Sector> Results { get; private set; } = new List<Sector>();

			public QuestInitializationFinder(ISymbol questTypeToFind, Code code)
				: base(code)
			{
				this.questTypeToFind = questTypeToFind;
			}

			public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
			{
				base.VisitObjectCreationExpression(node);

				var model = Code.Compilation.GetSemanticModel(node.SyntaxTree);

				if (model.GetTypeInfo(node).Type == questTypeToFind) {
					var sector = Code.SectorsAndCodeBulks[currentCodeBulk];
					Results.AddIfNotContains(sector);
				}
			}
		}

		public override CodeBulkType[] AcceptedTypes => new [] { CodeBulkType.Quest };

		public override void Read(CodeBulk codeBulk, Code code, ref Flow flow)
		{
			var classDecl = codeBulk.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();
			var sym = code.GetSymbolFor(classDecl, codeBulk);
			var visitor = new QuestInitializationFinder(sym, code);
			foreach (var sectorCode in code.SectorsCode) {
				visitor.Visit(sectorCode);
			}

			var quest = code.QuestsAndCodeBulks[codeBulk];
			var data = quest.Ensure<InitializationData>();
			data.InitializationPlaces.AddRange(visitor.Results);
		}
	}
}
