using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.CodeEditing.SyntaxRewriters;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Commands
{
	class RemoveQuestCommand : Command
	{
		private readonly Quest quest;
		private readonly Node node;
		private readonly CodeBulk codeBulk;

		private CodeSnapshot snapshot;

		public RemoveQuestCommand(Quest questToDelete, EditingContext context)
			: base(context)
		{
			quest = questToDelete;
			node = Context.Flow.Graph.FindNodeForQuest(quest);
			codeBulk = Context.Code.QuestsAndCodeBulks[quest];
		}

		public override void Do()
		{
			snapshot = new CodeSnapshot();

			var classDecls = codeBulk.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
			foreach (var classDecl in classDecls) {
				var typeRemover = new ClassConstructorCallRemover(
					Context.CodeEditor.GetSymbolFor(classDecl, codeBulk),
					Context.Code
				);
				snapshot.Merge(Context.CodeEditor.ApplySyntaxRewriters(typeRemover));
			}
			Context.Flow.Graph.RemoveNode(node);
			Context.Code.Remove(codeBulk);
		}

		public override void Undo()
		{
			Context.Code.Add(codeBulk);

			Context.CodeEditor.ApplySnapshot(snapshot);

			Context.Code.QuestsAndCodeBulks[quest] = codeBulk;

			Context.Flow.Graph.AddNode(node);
		}
	}
}
