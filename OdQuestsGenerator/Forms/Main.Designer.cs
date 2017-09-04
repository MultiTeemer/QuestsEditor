namespace OdQuestsGenerator.Forms
{
	partial class Main
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
			this.resultViewer = new System.Windows.Forms.RichTextBox();
			this.changeNameBtn = new System.Windows.Forms.Button();
			this.addStateButton = new System.Windows.Forms.Button();
			this.statesViewer = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// resultViewer
			// 
			this.resultViewer.Location = new System.Drawing.Point(21, 74);
			this.resultViewer.Name = "resultViewer";
			this.resultViewer.ReadOnly = true;
			this.resultViewer.Size = new System.Drawing.Size(844, 686);
			this.resultViewer.TabIndex = 0;
			this.resultViewer.Text = "";
			// 
			// changeNameBtn
			// 
			this.changeNameBtn.Location = new System.Drawing.Point(21, 13);
			this.changeNameBtn.Name = "changeNameBtn";
			this.changeNameBtn.Size = new System.Drawing.Size(150, 23);
			this.changeNameBtn.TabIndex = 1;
			this.changeNameBtn.Text = "Change name";
			this.changeNameBtn.UseVisualStyleBackColor = true;
			this.changeNameBtn.Click += new System.EventHandler(this.changeNameBtn_Click);
			// 
			// addStateButton
			// 
			this.addStateButton.Location = new System.Drawing.Point(896, 402);
			this.addStateButton.Name = "addStateButton";
			this.addStateButton.Size = new System.Drawing.Size(247, 44);
			this.addStateButton.TabIndex = 3;
			this.addStateButton.Text = "Add state";
			this.addStateButton.UseVisualStyleBackColor = true;
			this.addStateButton.Click += new System.EventHandler(this.addStateButton_Click);
			// 
			// statesViewer
			// 
			this.statesViewer.FormattingEnabled = true;
			this.statesViewer.Location = new System.Drawing.Point(896, 74);
			this.statesViewer.Name = "statesViewer";
			this.statesViewer.Size = new System.Drawing.Size(247, 316);
			this.statesViewer.TabIndex = 4;
			this.statesViewer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.statesViewer_MouseDoubleClick);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1202, 806);
			this.Controls.Add(this.statesViewer);
			this.Controls.Add(this.addStateButton);
			this.Controls.Add(this.changeNameBtn);
			this.Controls.Add(this.resultViewer);
			this.Name = "Main";
			this.Text = "Ocean Drop quests code generator";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox resultViewer;
		private System.Windows.Forms.Button changeNameBtn;
		private System.Windows.Forms.Button addStateButton;
		private System.Windows.Forms.ListBox statesViewer;
	}
}

