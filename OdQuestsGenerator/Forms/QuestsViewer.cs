using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataweb.NShape;
using Dataweb.NShape.GeneralShapes;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using OdQuestsGenerator.ApplicationData;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;
using OdQuestsGenerator.Forms.QuestsViewerStuff;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;
using OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms
{
	public partial class QuestsViewer : Form
	{
		private readonly Code code = new Code();
		private readonly CodeEditor editor;
		private readonly Loader loader;
		private readonly FlowView flowView;
		private readonly ShapeTemplatesFactory templates;
		private readonly ToolsManager toolsManager;
		private readonly CommandsHistory history = new CommandsHistory();

		private Flow flow;
		private Quest selectedQuest;
		private Sector selectedSector;
		private string selectedFolder;

		public QuestsViewer()
		{
			InitializeComponent();

			templates = new ShapeTemplatesFactory(project);
			loader = new Loader(code);
			editor = new CodeEditor(code);
			flowView = new FlowView(diagramSetController, project, display, templates);
			toolsManager = new ToolsManager(toolSetController);
		}

		private void LoadSectors()
		{
			//try {
				flow = loader.Load(selectedFolder);
				editor.Initialize();

				FillSectors();

				if (flow.Sectors.Count > 0) {
					SelectSector(0);
				}

				flowView.Display(flow.Graph);
			//} catch (Exception e) {
				//throw;
				//MessageBox.Show($"During operation performing exception has been thrown: {e.Message}");
			//}
		}

		private void LoadPreferences()
		{
			var path = GetPreferencesPath();
			if (File.Exists(path)) {
				var contents = FileSystem.ReadFileContents(path);
				Program.Preferences = JsonConvert.DeserializeObject<Preferences>(contents);
				selectedFolder = Program.Preferences.LastProjectPath;
				LoadSectors();
			}
		}

		private void SavePreferences()
		{
			if (!Directory.Exists(GetPreferencesDirPath())) {
				Directory.CreateDirectory(GetPreferencesDirPath());
			}

			File.WriteAllText(GetPreferencesPath(), JsonConvert.SerializeObject(Program.Preferences));
		}

		private string GetPreferencesPath() => Path.Combine(GetPreferencesDirPath(), "preferences.userprefs");

		private string GetPreferencesDirPath() => Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"OdQuestsEditor"
			);

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
				fbd.SelectedPath = selectedFolder;
				DialogResult result = fbd.ShowDialog();
				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath)) {
					selectedFolder = fbd.SelectedPath;
					Program.Preferences.LastProjectPath = selectedFolder;
					SavePreferences();
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
			InitProject();
			InitTools();
			LoadPreferences();
		}

		private void InitProject()
		{
			var programFilesDir = Environment.GetEnvironmentVariable(
				string.Format("ProgramFiles{0}", (IntPtr.Size == sizeof(long)) ? "(x86)" : "")
			);
			project.LibrarySearchPaths.Add(
				Path.Combine(programFilesDir, string.Format("dataweb{0}NShape{0}bin", Path.DirectorySeparatorChar))
			);

			project.AddLibrary(typeof(Ellipse).Assembly, false);

			project.Name = "ff";
			project.Create();
		}

		private void InitTools()
		{
			toolsManager.Clear();

			var context = new EditingContext(project, history, flowView, code, editor);

			toolsManager.AddTool(new SelectionTool(), new QuestsManipulatorWrapper(context));
			toolsManager.AddTool(
				new LinearShapeCreationTool(new Template("Link", templates.GetLinkTemplate())),
				new StubWrapper(context)
			);
			toolsManager.AddTool(
				new PlanarShapeCreationTool(new Template("Quest", templates.GetQuestTemplate())),
				new StubWrapper(context)
			);
		}
	}
}
