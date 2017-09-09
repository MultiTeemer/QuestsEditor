using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms
{
	public partial class QuestsViewer : Form
	{
		private Quest currentQuest;

		public QuestsViewer()
		{
			InitializeComponent();
		}

		private void QuestLoaded()
		{
			questNameLabel.Text = $"{currentQuest.Name} at {currentQuest.SectorName} (final state is {currentQuest.FinalState?.Name})";

			FillStates();
		}

		private void FillStates()
		{
			statesViewer.Items.Clear();
			statesViewer.Items.AddRange(currentQuest.States.Select(s => s.Name).ToArray());
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			openQuestFileDialog.ShowDialog(this);
		}

		private void openQuestFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var filePath = openQuestFileDialog.InitialDirectory + openQuestFileDialog.FileName;
			var content = FileSystem.ReadFileContents(filePath);
			questCode.Text = content;
			var tree = CSharpSyntaxTree.ParseText(content);
			currentQuest = FromCodeTransformer.FromCode(tree);
			QuestLoaded();
		}
	}
}
