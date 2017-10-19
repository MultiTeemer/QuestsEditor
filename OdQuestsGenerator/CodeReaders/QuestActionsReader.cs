using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using OdQuestsGenerator.CodeReaders.QuestActionsAnalysis;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.CodeReaders
{
	delegate QuestAction QuestActionAnalyzerDelegate(StatementSyntax expression);

	class QuestActionsReader : CodeReader
	{
		private static readonly List<QuestActionAnalyzerDelegate> analyzers = new List<QuestActionAnalyzerDelegate>();

		public override CodeBulkType[] AcceptedTypes => new[] { CodeBulkType.Quest };

		public QuestActionsReader()
		{
			var methods = typeof(Analyzers)
				.GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(QuestActionAnalyzerAttribute), inherit: false).Any())
				.Select(m => (QuestActionAnalyzerDelegate)Delegate.CreateDelegate(typeof(QuestActionAnalyzerDelegate), m));
			;
			analyzers.AddRange(methods);
		}

		public override void Read(CodeBulk codeBulk, Code code, ref Flow flow)
		{
			var quest = code.QuestsAndCodeBulks[codeBulk];
			var stateNameToMethod = codeBulk.Tree.GetAllStatesMethods();

			var nodesToAnnotatedNodes = new Dictionary<SyntaxNode, SyntaxNode>();

			foreach (var stateDesk in stateNameToMethod) {
				var state = quest.States.First(s => s.Name == stateDesk.Item1);
				var data = state.Ensure<QuestActionsData>();

				foreach (var statement in stateDesk.Item2.Body.Statements) {
					var wasInitialized = false;
					for (int i = 0; i < analyzers.Count && !wasInitialized; ++i) {
						var res = analyzers[i](statement);
						if (res != null) {
							data.Actions.Add(res);
							res.Source = statement.ToString();
							wasInitialized = true;
							nodesToAnnotatedNodes[statement] = statement.WithAdditionalAnnotations(new SyntaxAnnotation(res.Id));
						}
					}
					if (!wasInitialized) {
						var res = new QuestAction {
							Type = QuestActionType.NotParsed,
							Source = statement.ToString(),
						};
						data.Actions.Add(res);
						nodesToAnnotatedNodes[statement] = statement.WithAdditionalAnnotations(new SyntaxAnnotation(res.Id));
					}
				}
			}

			var editor = new SyntaxEditor(codeBulk.Tree.GetRoot(), code.Solution.Workspace);
			foreach (var kv in nodesToAnnotatedNodes) {
				editor.ReplaceNode(kv.Key, kv.Value);
			}
			codeBulk.Tree = editor.GetChangedRoot().SyntaxTree;
			codeBulk.WasModified = false;
		}
	}
}
