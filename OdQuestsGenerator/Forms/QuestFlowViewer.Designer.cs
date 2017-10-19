namespace OdQuestsGenerator.Forms
{
	partial class QuestFlowViewer
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuestFlowViewer));
			Dataweb.NShape.RoleBasedSecurityManager roleBasedSecurityManager1 = new Dataweb.NShape.RoleBasedSecurityManager();
			this.display = new Dataweb.NShape.WinFormsUI.Display();
			this.diagramSetController = new Dataweb.NShape.Controllers.DiagramSetController();
			this.project = new Dataweb.NShape.Project(this.components);
			this.cachedRepository = new Dataweb.NShape.Advanced.CachedRepository();
			this.toolSetListViewPresenter = new Dataweb.NShape.WinFormsUI.ToolSetListViewPresenter(this.components);
			this.toolsListView = new System.Windows.Forms.ListView();
			this.toolSetController = new Dataweb.NShape.Controllers.ToolSetController();
			this.SuspendLayout();
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
			this.display.Location = new System.Drawing.Point(12, 12);
			this.display.Name = "display";
			this.display.PropertyController = null;
			this.display.SelectionHilightColor = System.Drawing.Color.Firebrick;
			this.display.SelectionInactiveColor = System.Drawing.Color.Gray;
			this.display.SelectionInteriorColor = System.Drawing.Color.WhiteSmoke;
			this.display.SelectionNormalColor = System.Drawing.Color.DarkGreen;
			this.display.Size = new System.Drawing.Size(1025, 785);
			this.display.SnapToGrid = false;
			this.display.TabIndex = 0;
			this.display.ToolPreviewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
			this.display.ToolPreviewColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
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
			this.toolsListView.Location = new System.Drawing.Point(12, 12);
			this.toolsListView.MultiSelect = false;
			this.toolsListView.Name = "toolsListView";
			this.toolsListView.ShowItemToolTips = true;
			this.toolsListView.Size = new System.Drawing.Size(102, 199);
			this.toolsListView.TabIndex = 1;
			this.toolsListView.UseCompatibleStateImageBehavior = false;
			this.toolsListView.View = System.Windows.Forms.View.SmallIcon;
			// 
			// toolSetController
			// 
			this.toolSetController.DiagramSetController = this.diagramSetController;
			// 
			// QuestFlowViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1434, 803);
			this.Controls.Add(this.toolsListView);
			this.Controls.Add(this.display);
			this.Name = "QuestFlowViewer";
			this.Text = "QuestFlowViewer";
			this.ResumeLayout(false);

		}

		#endregion

		private Dataweb.NShape.WinFormsUI.Display display;
		private Dataweb.NShape.WinFormsUI.ToolSetListViewPresenter toolSetListViewPresenter;
		private System.Windows.Forms.ListView toolsListView;
		private Dataweb.NShape.Controllers.ToolSetController toolSetController;
		private Dataweb.NShape.Controllers.DiagramSetController diagramSetController;
		private Dataweb.NShape.Project project;
		private Dataweb.NShape.Advanced.CachedRepository cachedRepository;
	}
}