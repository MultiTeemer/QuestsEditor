namespace OdQuestsGenerator.Forms
{
	partial class QuestState
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
			this.label1 = new System.Windows.Forms.Label();
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.saveButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.isFinalCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(30, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// nameTextBox
			// 
			this.nameTextBox.Location = new System.Drawing.Point(100, 17);
			this.nameTextBox.Name = "nameTextBox";
			this.nameTextBox.Size = new System.Drawing.Size(483, 20);
			this.nameTextBox.TabIndex = 1;
			this.nameTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.nameTextBox_KeyUp);
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(201, 84);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(104, 22);
			this.saveButton.TabIndex = 2;
			this.saveButton.Text = "Save";
			this.saveButton.UseVisualStyleBackColor = true;
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.Location = new System.Drawing.Point(323, 85);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(100, 20);
			this.deleteButton.TabIndex = 3;
			this.deleteButton.Text = "Delete";
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(33, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Is final:";
			// 
			// isFinalCheckBox
			// 
			this.isFinalCheckBox.AutoSize = true;
			this.isFinalCheckBox.Location = new System.Drawing.Point(100, 61);
			this.isFinalCheckBox.Name = "isFinalCheckBox";
			this.isFinalCheckBox.Size = new System.Drawing.Size(15, 14);
			this.isFinalCheckBox.TabIndex = 5;
			this.isFinalCheckBox.UseVisualStyleBackColor = true;
			this.isFinalCheckBox.CheckedChanged += new System.EventHandler(this.isFinalCheckBox_CheckedChanged);
			// 
			// QuestState
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(602, 118);
			this.Controls.Add(this.isFinalCheckBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.nameTextBox);
			this.Controls.Add(this.label1);
			this.Name = "QuestState";
			this.Text = "QuestState";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox isFinalCheckBox;
	}
}