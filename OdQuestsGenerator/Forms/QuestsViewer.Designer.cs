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
			this.openMenuStrip = new System.Windows.Forms.MenuStrip();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openQuestFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.questCode = new System.Windows.Forms.RichTextBox();
			this.statesViewer = new System.Windows.Forms.ListBox();
			this.questNameLabel = new System.Windows.Forms.Label();
			this.sectorsViewer = new System.Windows.Forms.ListBox();
			this.questsListBox = new System.Windows.Forms.ListBox();
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
			// questCode
			// 
			this.questCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.questCode.Location = new System.Drawing.Point(233, 48);
			this.questCode.Name = "questCode";
			this.questCode.Size = new System.Drawing.Size(610, 641);
			this.questCode.TabIndex = 1;
			this.questCode.Text = "";
			// 
			// statesViewer
			// 
			this.statesViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.statesViewer.FormattingEnabled = true;
			this.statesViewer.Location = new System.Drawing.Point(852, 89);
			this.statesViewer.Name = "statesViewer";
			this.statesViewer.Size = new System.Drawing.Size(176, 589);
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
			this.sectorsViewer.Location = new System.Drawing.Point(9, 50);
			this.sectorsViewer.Name = "sectorsViewer";
			this.sectorsViewer.Size = new System.Drawing.Size(212, 277);
			this.sectorsViewer.TabIndex = 4;
			this.sectorsViewer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.sectorsViewer_MouseClick);
			// 
			// questsListBox
			// 
			this.questsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.questsListBox.FormattingEnabled = true;
			this.questsListBox.Location = new System.Drawing.Point(11, 352);
			this.questsListBox.Name = "questsListBox";
			this.questsListBox.Size = new System.Drawing.Size(209, 342);
			this.questsListBox.TabIndex = 5;
			this.questsListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.questsListBox_MouseClick);
			// 
			// QuestsViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(1048, 772);
			this.Controls.Add(this.questsListBox);
			this.Controls.Add(this.sectorsViewer);
			this.Controls.Add(this.questNameLabel);
			this.Controls.Add(this.statesViewer);
			this.Controls.Add(this.questCode);
			this.Controls.Add(this.openMenuStrip);
			this.MainMenuStrip = this.openMenuStrip;
			this.Name = "QuestsViewer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "QuestsViewer";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.openMenuStrip.ResumeLayout(false);
			this.openMenuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip openMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.OpenFileDialog openQuestFileDialog;
		private System.Windows.Forms.RichTextBox questCode;
		private System.Windows.Forms.ListBox statesViewer;
		private System.Windows.Forms.Label questNameLabel;
		private System.Windows.Forms.ListBox sectorsViewer;
		private System.Windows.Forms.ListBox questsListBox;
	}
}