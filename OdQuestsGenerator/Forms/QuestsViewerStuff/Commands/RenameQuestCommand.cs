using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Commands
{
	class RenameQuestCommand : Command
	{
		private readonly Quest quest;
		private readonly string oldName;
		private readonly string newName;

		public RenameQuestCommand(Quest quest, string newName, EditingContext context)
			: base(context)
		{
			this.quest = quest;
			this.newName = newName;

			oldName = quest.Name;
		}

		public override void Do()
		{
			quest.Name = newName;

			SetViewName(newName);
			RenameQuestInCode(oldName, newName);
		}

		public override void Undo()
		{
			quest.Name = oldName;

			SetViewName(oldName);
			RenameQuestInCode(newName, oldName);
		}

		private void SetViewName(string name)
		{
			var shape = Context.FlowView.GetShapeForQuest(quest);
			shape.SetCaptionText(0, name);
		}

		private void RenameQuestInCode(string oldName, string newName)
		{
			var codeBulk = Context.Code.GetCodeForQuest(quest);
			var enumDecl = codeBulk.Tree.GetRoot().GetFirstOfType<EnumDeclarationSyntax>();
			var componentDecl = codeBulk.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();
			var questDecl = codeBulk.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Last();

			var componentName = newName;
			var enumName = $"{newName}QuestState";
			var questName = $"{newName}Quest";

			Context.CodeEditor.Rename(codeBulk, questDecl, questName);
			Context.CodeEditor.Rename(codeBulk, componentDecl, componentName);
			Context.CodeEditor.Rename(codeBulk, enumDecl, enumName);

			Context.Code.RenameFile(codeBulk, $"{questName}.cs");
		}
	}
}
