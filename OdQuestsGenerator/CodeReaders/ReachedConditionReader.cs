using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.CodeReaders.SyntaxVisitors;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.CodeReaders
{
	class NotEditableLinks : IData {}

	public static class NotEditableLinksQuestExtensions
	{
		public static bool IsLinksToEditable(this Quest quest) => quest.Data.Records.FirstOfTypeOrDefault<NotEditableLinks>() == null;
	}

	class ReachedConditionReader : CodeReader
	{
		private class LinksReader : SyntaxVisitor
		{
			private class QuestNameFetcher : SyntaxVisitor
			{
				public readonly List<string> Results = new List<string>();

				public QuestNameFetcher(Code code)
					: base(code)
				{}

				public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
				{
					base.VisitMemberAccessExpression(node);

					if (node.Expression is MemberAccessExpressionSyntax left) {
						var model = Code.Compilation.GetSemanticModel(node.SyntaxTree);
						var type = model.GetTypeInfo(left.Expression).Type;
						var questName = CodeEditor.FromQuestClassNametoQuestName(type.Name);

						if (!Results.Contains(questName)) {
							Results.Add(questName);
						}
					}
				}
			}

			public readonly List<Tuple<string, string>> Results = new List<Tuple<string, string>>();
			public readonly List<string> NotEditableResults = new List<string>();

			public LinksReader(Code code)
				: base(code)
			{}

			public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
			{
				base.VisitObjectCreationExpression(node);

				var linkExpr = node.Initializer?.FindLinkExpression();
				if (linkExpr != null) {
					var model = Code.Compilation.GetSemanticModel(node.SyntaxTree);
					var linkedQuestType = model.GetTypeInfo(node).Type;
					var linkedQuestName = CodeEditor.FromQuestClassNametoQuestName(linkedQuestType.Name);

					if (linkExpr.Right.IsEditableInterQuestLinkExpression()) {
						var fetcher = new QuestNameFetcher(Code);
						fetcher.Visit(linkExpr.Right);
						foreach (var r in fetcher.Results) {
							Results.Add(Tuple.Create(r, linkedQuestName));
						}
					} else {
						NotEditableResults.Add(linkedQuestName);
					}
				}
			}
		}

		public override CodeBulkType[] AcceptedTypes => new[] { CodeBulkType.Sector };

		public override void Read(CodeBulk codeBulk, Code code, ref Flow flow)
		{
			var linksReader = new LinksReader(code);
			linksReader.Visit(codeBulk);
			var links = linksReader.Results;
			foreach (var link in links) {
				var n1 = flow.Graph.FindNodeForQuest(link.Item1);
				var n2 = flow.Graph.FindNodeForQuest(link.Item2);

				if (n1 == null) {
					throw new Exception($"Cannot find node for {link.Item1}");
				}

				if (n2 == null) {
					throw new Exception($"Cannot find node for {link.Item2}");
				}

				flow.Graph.AddLink(n1, n2);
			}
			foreach (var notEditableQuest in linksReader.NotEditableResults) {
				var n = flow.Graph.FindNodeForQuest(notEditableQuest);
				n.Quest.Data.Ensure<NotEditableLinks>();
			}
		}
	}
}
