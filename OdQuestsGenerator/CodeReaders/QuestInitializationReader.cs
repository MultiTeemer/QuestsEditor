using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.CodeReaders
{
	class ActivationData : IData
	{
		public List<Sector> Sectors { get; set; } = new List<Sector>();

		public ActivationData Clone() => new ActivationData {
			Sectors = Sectors.Select(p => p).ToList(),
		};
	}

	public static class ActivationDataQuestExtensions
	{
		public static bool IsActive(this Quest quest)
		{
			var initData = quest.Data.Records.FirstOfTypeOrDefault<ActivationData>();
			return initData != null && initData.Sectors.Count > 0;
		}
	}

	class QuestActivationReader : CodeReader
	{
		private class QuestInitializationFinder : SyntaxVisitors.SyntaxWalker
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
					var sector = Code.SectorsAndCodeBulks[CurrentCodeBulk];
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
			var data = quest.Data.Ensure<ActivationData>();
			data.Sectors.AddRange(visitor.Results);
		}
	}
}
