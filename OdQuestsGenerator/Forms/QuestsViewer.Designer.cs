using System.Reflection;

namespace OdQuestsGenerator.Forms
{
	partial class QuestsViewer
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuestsViewer));
			Dataweb.NShape.RoleBasedSecurityManager roleBasedSecurityManager1 = new Dataweb.NShape.RoleBasedSecurityManager();
			this.openMenuStrip = new System.Windows.Forms.MenuStrip();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openQuestFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.statesViewer = new System.Windows.Forms.ListBox();
			this.questNameLabel = new System.Windows.Forms.Label();
			this.sectorsViewer = new System.Windows.Forms.ListBox();
			this.questsListBox = new System.Windows.Forms.ListBox();
			this.toolSetController = new Dataweb.NShape.Controllers.ToolSetController();
			this.diagramSetController = new Dataweb.NShape.Controllers.DiagramSetController();
			this.project = new Dataweb.NShape.Project(this.components);
			this.cachedRepository = new Dataweb.NShape.Advanced.CachedRepository();
			this.toolSetListViewPresenter = new Dataweb.NShape.WinFormsUI.ToolSetListViewPresenter(this.components);
			this.toolsListView = new System.Windows.Forms.ListView();
			this.display = new Dataweb.NShape.WinFormsUI.Display();
			this.openMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// openMenuStrip
			// 
			this.openMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
			this.openMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.openMenuStrip.Name = "openMenuStrip";
			this.openMenuStrip.Size = new System.Drawing.Size(1048, 24);
			this.openMenuStrip.TabIndex = 0;
			this.openMenuStrip.Text = "openMenuStrip";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.openToolStripMenuItem.Text = "Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// openQuestFileDialog
			// 
			this.openQuestFileDialog.FileName = "openQuestFileDialog";
			this.openQuestFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openQuestFileDialog_FileOk);
			// 
			// statesViewer
			// 
			this.statesViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.statesViewer.FormattingEnabled = true;
			this.statesViewer.Location = new System.Drawing.Point(849, 27);
			this.statesViewer.Name = "statesViewer";
			this.statesViewer.Size = new System.Drawing.Size(176, 212);
			this.statesViewer.TabIndex = 2;
			// 
			// questNameLabel
			// 
			this.questNameLabel.AutoSize = true;
			this.questNameLabel.Location = new System.Drawing.Point(849, 48);
			this.questNameLabel.Name = "questNameLabel";
			this.questNameLabel.Size = new System.Drawing.Size(0, 13);
			this.questNameLabel.TabIndex = 3;
			// 
			// sectorsViewer
			// 
			this.sectorsViewer.FormattingEnabled = true;
			this.sectorsViewer.Location = new System.Drawing.Point(8, 27);
			this.sectorsViewer.Name = "sectorsViewer";
			this.sectorsViewer.Size = new System.Drawing.Size(212, 69);
			this.sectorsViewer.TabIndex = 4;
			this.sectorsViewer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.sectorsViewer_MouseClick);
			// 
			// questsListBox
			// 
			this.questsListBox.FormattingEnabled = true;
			this.questsListBox.Location = new System.Drawing.Point(8, 116);
			this.questsListBox.Name = "questsListBox";
			this.questsListBox.Size = new System.Drawing.Size(212, 251);
			this.questsListBox.TabIndex = 5;
			this.questsListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.questsListBox_MouseClick);
			// 
			// toolSetController
			// 
			this.toolSetController.DiagramSetController = this.diagramSetController;
			// 
			// diagramSetController
			// 
			this.diagramSetController.ActiveTool = null;
			this.diagramSetController.Project = this.project;
			// 
			// project
			// 
			this.project.Description = null;
			this.project.LibrarySearchPaths = ((System.Collections.Generic.IList<string>)(resources.GetObject("project.LibrarySearchPaths")));
			this.project.Name = null;
			this.project.Repository = this.cachedRepository;
			roleBasedSecurityManager1.CurrentRole = Dataweb.NShape.StandardRole.Administrator;
			roleBasedSecurityManager1.CurrentRoleName = "Administrator";
			this.project.SecurityManager = roleBasedSecurityManager1;
			// 
			// cachedRepository
			// 
			this.cachedRepository.ProjectName = null;
			this.cachedRepository.Store = null;
			this.cachedRepository.Version = 0;
			// 
			// toolSetListViewPresenter
			// 
			this.toolSetListViewPresenter.HideDeniedMenuItems = false;
			this.toolSetListViewPresenter.ListView = this.toolsListView;
			this.toolSetListViewPresenter.ShowDefaultContextMenu = true;
			this.toolSetListViewPresenter.ToolSetController = this.toolSetController;
			// 
			// toolsListView
			// 
			this.toolsListView.FullRowSelect = true;
			this.toolsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.toolsListView.HideSelection = false;
			this.toolsListView.Location = new System.Drawing.Point(226, 27);
			this.toolsListView.MultiSelect = false;
			this.toolsListView.Name = "toolsListView";
			this.toolsListView.ShowItemToolTips = true;
			this.toolsListView.Size = new System.Drawing.Size(76, 129);
			this.toolsListView.TabIndex = 10;
			this.toolsListView.UseCompatibleStateImageBehavior = false;
			this.toolsListView.View = System.Windows.Forms.View.List;
			// 
			// display
			// 
			this.display.AllowDrop = true;
			this.display.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.display.BackColorGradient = System.Drawing.SystemColors.Control;
			this.display.DiagramSetController = this.diagramSetController;
			this.display.GridColor = System.Drawing.Color.Gainsboro;
			this.display.GridSize = 19;
			this.display.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.display.Location = new System.Drawing.Point(226, 27);
			this.display.Name = "display";
			this.display.PropertyController = null;
			this.display.SelectionHilightColor = System.Drawing.Color.Firebrick;
			this.display.SelectionInactiveColor = System.Drawing.Color.Gray;
			this.display.SelectionInteriorColor = System.Drawing.Color.WhiteSmoke;
			this.display.SelectionNormalColor = System.Drawing.Color.DarkGreen;
			this.display.Size = new System.Drawing.Size(607, 700);
			this.display.TabIndex = 9;
			this.display.ToolPreviewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
			this.display.ToolPreviewColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
			// 
			// QuestsViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(1048, 772);
			this.Controls.Add(this.toolsListView);
			this.Controls.Add(this.questsListBox);
			this.Controls.Add(this.display);
			this.Controls.Add(this.sectorsViewer);
			this.Controls.Add(this.questNameLabel);
			this.Controls.Add(this.statesViewer);
			this.Controls.Add(this.openMenuStrip);
			this.MainMenuStrip = this.openMenuStrip;
			this.Name = "QuestsViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "OD Quests Editor";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.QuestsViewer_Load);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.QuestsViewer_KeyUp);
			this.openMenuStrip.ResumeLayout(false);
			this.openMenuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip openMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog openQuestFileDialog;
		private System.Windows.Forms.ListBox statesViewer;
		private System.Windows.Forms.Label questNameLabel;
		private System.Windows.Forms.ListBox sectorsViewer;
		private System.Windows.Forms.ListBox questsListBox;
		private Dataweb.NShape.Controllers.ToolSetController toolSetController;
		private Dataweb.NShape.Controllers.DiagramSetController diagramSetController;
		private Dataweb.NShape.Project project;
		private Dataweb.NShape.Advanced.CachedRepository cachedRepository;
		private Dataweb.NShape.WinFormsUI.ToolSetListViewPresenter toolSetListViewPresenter;
		private System.Windows.Forms.ListView toolsListView;
		private Dataweb.NShape.WinFormsUI.Display display;
	}
}