using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.CodeAnalysis.CSharp;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms
{
	public partial class QuestsViewer : Form
	{
		private readonly List<Sector> sectors = new List<Sector>();

		private Quest selectedQuest;
		private Sector selectedSector;
		private string selectedFolder;

		public QuestsViewer()
		{
			InitializeComponent();
		}

		private void LoadSectors()
		{
			var gameDir = Directory.EnumerateDirectories(selectedFolder).FirstOrDefault(p => p.EndsWith(".Game"));
			if (gameDir == null) {
				MessageBox.Show("Couldn't find \"*.Game\" directory");
				return;
			}

			var sectorsDir = Path.Combine(gameDir, "Sectors");
			if (!Directory.Exists(sectorsDir)) {
				MessageBox.Show($"Couldn't find \"{sectorsDir}\" directory");
				return;
			}

			var sectorsDirs = Directory.EnumerateDirectories(sectorsDir)
				.Where(p => new DirectoryInfo(p).Name.StartsWith("Sector"));
			foreach (var dir in sectorsDirs) {
				LoadSector(dir);
			}

			FillSectors();

			if (sectors.Count > 0) {
				SelectSector(0);
			}
		}

		private void LoadSector(string dirPath)
		{
			var sectorFile = Directory.EnumerateFiles(dirPath)
				.FirstOrDefault(p => new FileInfo(p).Name.StartsWith("Sector"));
			if (sectorFile == null) {
				return;
			}

			var tree = FileSystem.ReadCodeFromFile(Path.Combine(dirPath, sectorFile));
			var sector = new Sector {
				Name = FromCodeTransformer.FetchSectorName(tree),
				Quests = new List<Quest>(),
			};

			var questsDir = Path.Combine(dirPath, "Quests");
			if (Directory.Exists(questsDir)) {
				var files = Directory.EnumerateFiles(questsDir);
				foreach (var filePath in files) {
					sector.Quests.Add(LoadQuest(filePath));
				}
			}

			sectors.Add(sector);
		}

		private Quest LoadQuest(string path)
		{
			var tree = FileSystem.ReadCodeFromFile(path);
			var quest = FromCodeTransformer.ReadQuest(tree);

			return quest;
		}

		private void SelectSector(int idx)
		{
			selectedSector = sectors[idx];
			SectorSelected();
		}

		private void SelectSector(string name)
		{
			var sectorToSelect = sectors.FirstOrDefault(s => s.Name == name);
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
			questCode.Clear();
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
			var sectorsItems = sectors.Select(s => s.Name).ToArray();
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
			questCode.Text = content;
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
	}
}
