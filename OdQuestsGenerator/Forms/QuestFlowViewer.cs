using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing;
using OdQuestsGenerator.Forms.QuestFlowViewerStuff;
using OdQuestsGenerator.Forms.QuestFlowViewerStuff.ToolsWrappers;

namespace OdQuestsGenerator.Forms
{
	public partial class QuestFlowViewer : Form
	{
		private readonly Quest quest;
		private readonly QuestFlowView flowView;
		private readonly EditingContext context;
		private readonly ToolsManager toolsManager;
		private readonly ShortcutsReader shortcutsReader;

		private int lastSavedActionIdx = -1;

		internal QuestFlowViewer(Quest quest, EditingContext context)
		{
			this.quest = quest;
			this.context = context;

			InitializeComponent();

			flowView = new QuestFlowView(diagramSetController, project, display);
			toolsManager = new ToolsManager(toolSetController, flowView);
			shortcutsReader = new ShortcutsReader(context.Code, context.History);

			context.Code.Saved += Code_Saved;

			context.History.Done += History_Done;
			context.History.Undone += History_Undone;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			context.Code.Saved -= Code_Saved;

			context.History.Done -= History_Done;
			context.History.Undone -= History_Undone;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			InitProject();
			InitTools();

			flowView.DisplayFlow(quest);
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) =>
			shortcutsReader.ProcessCmdKey(ref msg, keyData) ?? base.ProcessCmdKey(ref msg, keyData);

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

			var tool1 = new BaseUIStuff.OverloadedTools.SelectionTool();
			toolsManager.AddTool(tool1, new QuestActionManipulatorWrapper(context, tool1, flowView));
		}

		private void UpdateTitle()
		{
			var title = $"{quest.Name} Flow";
			if (context.History.LastPerformedCommandIdx != lastSavedActionIdx) {
				title += " (NOT SAVED)";
			}

			Text = title;
		}


		private void History_Undone(ICommand obj)
		{
			UpdateTitle();
		}

		private void History_Done(ICommand arg1, bool arg2)
		{
			UpdateTitle();
		}

		private void Code_Saved()
		{
			lastSavedActionIdx = context.History.LastPerformedCommandIdx;

			UpdateTitle();
		}
	}
}
