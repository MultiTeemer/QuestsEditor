using System.Collections.Generic;
using System.Linq;
using Dataweb.NShape;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.SyntaxRewriters;
using OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Commands
{
	class RemoveQuestCommand : Command
	{
		private readonly Quest quest;
		private readonly Node node;
		private readonly CodeBulk codeBulk;
		private readonly Shape shape;

		private CodeSnapshot snapshot;

		public RemoveQuestCommand(Quest questToDelete, EditingContext context)
			: base(context)
		{
			quest = questToDelete;
			node = Context.Flow.Graph.FindNodeForQuest(quest);
			codeBulk = Context.Code.GetCodeForQuest(quest);
			shape = Context.FlowView.GetShapeForQuest(quest);
		}

		public override void Do()
		{
			snapshot = new CodeSnapshot();

			var classDecls = codeBulk.Tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
			foreach (var classDecl in classDecls) {
				var typeRemover = new ClassConstructorCallRemover(
					Context.CodeEditor.GetSymbolFor(classDecl, codeBulk),
					Context.CodeEditor.Solution,
					Context.CodeEditor.Compilation
				);
				snapshot.Merge(Context.CodeEditor.ApplySyntaxRewriters(typeRemover));
			}
			Context.Flow.Graph.RemoveNode(node);
			Context.CodeEditor.Remove(codeBulk);

			var shape = Context.FlowView.GetShapeForQuest(quest);
			if (shape != null) {
				Context.FlowView.RemoveNodeShape(shape);
			}
		}

		public override void Undo()
		{
			Context.CodeEditor.Add(codeBulk);
			Context.CodeEditor.ApplySnapshot(snapshot);

			Context.Code.RegisterQuestforCodeBulk(quest, codeBulk);

			Context.FlowView.AddShapeForNode(node, shape);

			Context.Flow.Graph.AddNode(node);
		}
	}
}
