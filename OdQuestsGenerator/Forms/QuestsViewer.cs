using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.CodeAnalysis.CSharp;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;
using OdQuestsGenerator.Utils;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.Forms.QuestsViewerStuff;

namespace OdQuestsGenerator.Forms
{
	public partial class QuestsViewer : Form
	{
		private readonly SectorsLoader loader = new SectorsLoader();
		private readonly SectorsFlowScheme flowScheme;

		private Flow flow;
		private Quest selectedQuest;
		private Sector selectedSector;
		private string selectedFolder;

		public QuestsViewer()
		{
			InitializeComponent();

			flowScheme = new SectorsFlowScheme(diagramSetController, project, display);
		}

		private void LoadSectors()
		{
			//try {
				flow = loader.Load(selectedFolder);

				FillSectors();

				if (flow.Sectors.Count > 0) {
					SelectSector(0);
				}

				flowScheme.Display(flow.Graph);
			//} catch (Exception e) {
				//throw;
				//MessageBox.Show($"During operation performing exception has been thrown: {e.Message}");
			//}
		}

		private void SelectSector(int idx)
		{
			selectedSector = flow.Sectors[idx];
			SectorSelected();
		}

		private void SelectSector(string name)
		{
			var sectorToSelect = flow.Sectors.FirstOrDefault(s => s.Name == name);
			if (sectorToSelect == null) {
				MessageBox.Show($"Couldn't find sector with name {name}");
				return;
			}

			selectedSector = sectorToSelect;
			SectorSelected();
		}

		private void SelectQuest(int idx)
		{
			selectedQuest = selectedSector.Quests[idx];
			QuestSelected();
		}

		private void SectorSelected()
		{
			FillQuests();

			selectedQuest = null;
			statesViewer.Items.Clear();
		}

		private void QuestSelected()
		{
			if (selectedQuest == null) {
				questNameLabel.Text = "";
				statesViewer.Items.Clear();
			} else {
				questNameLabel.Text = $"{selectedQuest.Name}";

				FillStates();
			}
		}

		private void FillStates()
		{
			var states = selectedQuest.States.Select(s => s.Name).ToArray();
			statesViewer.Fill(states);
		}

		private void FillSectors()
		{
			var sectorsItems = flow.Sectors.Select(s => s.Name).ToArray();
			sectorsViewer.Fill(sectorsItems);
		}

		private void FillQuests()
		{
			var quests = selectedSector.Quests.Select(q => q.Name).ToArray();
			questsListBox.Fill(quests);
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog()) {
				fbd.SelectedPath = Environment.CurrentDirectory;
				DialogResult result = fbd.ShowDialog();
				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
					selectedFolder = fbd.SelectedPath;
					LoadSectors();
				}
			}
		}

		private void openQuestFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			var filePath = openQuestFileDialog.InitialDirectory + openQuestFileDialog.FileName;
			var content = FileSystem.ReadFileContents(filePath);
			var tree = CSharpSyntaxTree.ParseText(content);
			selectedQuest = FromCodeTransformer.ReadQuest(tree);
			QuestSelected();
		}

		private void sectorsViewer_MouseClick(object sender, MouseEventArgs e)
		{
			int index = sectorsViewer.IndexFromPoint(e.Location);
			if (index != ListBox.NoMatches) {
				SelectSector(index);
			}
		}

		private void questsListBox_MouseClick(object sender, MouseEventArgs e)
		{
			int index = questsListBox.IndexFromPoint(e.Location);
			if (index != ListBox.NoMatches) {
				SelectQuest(index);
			}
		}

		private void QuestsViewer_Load(object sender, EventArgs e)
		{
			string programFilesDir = Environment.GetEnvironmentVariable(string.Format("ProgramFiles{0}", (IntPtr.Size == sizeof(long)) ? "(x86)" : ""));
			project.LibrarySearchPaths.Add(Path.Combine(programFilesDir, string.Format("dataweb{0}NShape{0}bin", Path.DirectorySeparatorChar)));

			// Add general shapes library and define that it should not be unloaded when closing the project
			project.AddLibrary(typeof(Ellipse).Assembly, false);

			project.Name = "ff";
			// Create a new NShape project
			project.Create();
		}
	}
}
