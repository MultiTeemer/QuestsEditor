using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OdQuestsGenerator.CodeEditing;
using OdQuestsGenerator.CodeReaders;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Commands
{
	class ActivateQuestCommand : Command
	{
		private readonly Quest quest;
		private readonly Sector sector;

		private CodeSnapshot snapshot;

		public ActivateQuestCommand(Quest quest, Sector sector, EditingContext context)
			: base(context)
		{
			this.quest = quest;
			this.sector = sector;
		}

		public override void Do()
		{
			var cb = Context.Code.SectorsAndCodeBulks[sector];

			snapshot = new CodeSnapshot();
			snapshot.PreviousCode[cb] = cb.Tree;

			var initializeMethod = cb.Tree
				.GetRoot()
				.DescendantNodes()
				.OfType<MethodDeclarationSyntax>()
				.First(m => m.Identifier.ValueText == "Initialize");
			var body = initializeMethod.Body;
			var statementText = $"var {CodeEditor.FormatQuestNameForVar(quest.Name)} = new {CodeEditor.FormatQuestNameForClass(quest.Name)} {{}};\n";
			var newInitializeMethod = initializeMethod.WithBody(body.AddStatements(SyntaxFactory.ParseStatement(statementText)));
			var newTree = cb.Tree.GetRoot().ReplaceNode(initializeMethod, newInitializeMethod).SyntaxTree;

			cb.Tree = newTree;

			var data = quest.Ensure<InitializationData>();
			data.InitializationPlaces.Add(sector);
		}

		public override void Undo()
		{
			Context.CodeEditor.ApplySnapshot(snapshot);

			var data = quest.Data.FirstOfType<InitializationData>();
			data.InitializationPlaces.Remove(sector);
		}
	}
}
