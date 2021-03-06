﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.CodeReaders.SyntaxVisitors;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Commands
{
	class RenameQuestCommand : Command
	{
		class LocalVarsFinder : CodeReaders.SyntaxVisitors.SyntaxWalker
		{
			public readonly Dictionary<VariableDeclaratorSyntax, CodeBulk> Results = new Dictionary<VariableDeclaratorSyntax, CodeBulk>();

			private readonly ISymbol type;

			public LocalVarsFinder(Code code, ISymbol type)
				: base(code)
			{
				this.type = type;
			}

			public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
			{
				base.VisitLocalDeclarationStatement(node);

				if (node.Declaration.Variables.Count == 1) {
					var model = Code.Compilation.GetSemanticModel(node.SyntaxTree);
					var decl = node.Declaration.Variables.First();
					var val = decl.Initializer?.Value;
					if (ReferenceEquals(model.GetTypeInfo(val).Type, type)) {
						Results[decl] = CurrentCodeBulk;
					}
				}
			}
		}

		private readonly Quest quest;
		private readonly string oldName;
		private readonly string newName;

		public RenameQuestCommand(Quest quest, string oldName, string newName, EditingContext context)
			: base(context)
		{
			this.quest = quest;
			this.newName = newName;
			this.oldName = oldName;
		}

		public override void Do()
		{
			quest.Name = newName;

			RenameQuestInCode(newName);
		}

		public override void Undo()
		{
			quest.Name = oldName;

			RenameQuestInCode(oldName);
		}

		private void RenameQuestInCode(string name)
		{
			var codeBulk = Context.Code.QuestsAndCodeBulks[quest];
			var enumDecl = codeBulk.Tree.GetRoot().GetFirstOfType<EnumDeclarationSyntax>();
			var componentDecl = codeBulk.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
			var questDecl = codeBulk.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

			var componentName = name;
			var enumName = $"{name}QuestState";
			var questName = $"{name}Quest";

			var finder = new LocalVarsFinder(Context.Code, Context.CodeEditor.GetSymbolFor(questDecl, codeBulk));
			foreach (var sectorCode in Context.Code.SectorsCode) {
				finder.Visit(sectorCode);
			}
			foreach (var kv in finder.Results) {
				Context.CodeEditor.Rename(kv.Value, kv.Key, CodeEditor.FormatQuestNameForVar(name));
			}

			Context.CodeEditor.Rename(codeBulk, questDecl, questName);
			Context.CodeEditor.Rename(codeBulk, componentDecl, componentName);
			Context.CodeEditor.Rename(codeBulk, enumDecl, enumName);

			Context.Code.RenameFile(codeBulk, $"{questName}.cs");
		}
	}
}
