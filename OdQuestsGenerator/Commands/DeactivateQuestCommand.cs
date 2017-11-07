using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.CodeEditing.SyntaxRewriters;
using OdQuestsGenerator.CodeReaders;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Commands
{
	class DeactivateQuestCommand : Command
	{
		private readonly CodeBulk codeBulk;
		private readonly Quest quest;
		private readonly ActivationData activationDataClone;

		private CodeSnapshot snapshot;

		public DeactivateQuestCommand(Quest quest, EditingContext context)
			: base(context)
		{
			this.quest = quest;

			activationDataClone = quest.Data.Ensure<ActivationData>().Clone();
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

			quest.Data.Ensure<ActivationData>().Sectors.Clear();
		}

		public override void Undo()
		{
			Context.CodeEditor.ApplySnapshot(snapshot);

			quest.Data.Ensure<ActivationData>().Sectors.AddRange(activationDataClone.Sectors);
		}
	}
}
