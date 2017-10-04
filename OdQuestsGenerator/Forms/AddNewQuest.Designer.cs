namespace OdQuestsGenerator.Forms
{
	partial class AddNewQuest
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
			this.addBtn = new System.Windows.Forms.Button();
			this.cancelBtn = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.sectorsComboBox = new System.Windows.Forms.ComboBox();
			this.questNameTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.activateCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(34, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Quest name:";
			// 
			// addBtn
			// 
			this.addBtn.Location = new System.Drawing.Point(37, 194);
			this.addBtn.Name = "addBtn";
			this.addBtn.Size = new System.Drawing.Size(176, 54);
			this.addBtn.TabIndex = 1;
			this.addBtn.Text = "Add";
			this.addBtn.UseVisualStyleBackColor = true;
			this.addBtn.Click += new System.EventHandler(this.addBtn_Click);
			// 
			// cancelBtn
			// 
			this.cancelBtn.Location = new System.Drawing.Point(237, 194);
			this.cancelBtn.Name = "cancelBtn";
			this.cancelBtn.Size = new System.Drawing.Size(184, 54);
			this.cancelBtn.TabIndex = 2;
			this.cancelBtn.Text = "Cancel";
			this.cancelBtn.UseVisualStyleBackColor = true;
			this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(34, 83);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Select sector:";
			// 
			// sectorsComboBox
			// 
			this.sectorsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.sectorsComboBox.FormattingEnabled = true;
			this.sectorsComboBox.Location = new System.Drawing.Point(146, 80);
			this.sectorsComboBox.Name = "sectorsComboBox";
			this.sectorsComboBox.Size = new System.Drawing.Size(275, 21);
			this.sectorsComboBox.TabIndex = 4;
			this.sectorsComboBox.SelectedValueChanged += new System.EventHandler(this.sectorsComboBox_SelectedValueChanged);
			// 
			// questNameTextBox
			// 
			this.questNameTextBox.Location = new System.Drawing.Point(146, 28);
			this.questNameTextBox.Name = "questNameTextBox";
			this.questNameTextBox.Size = new System.Drawing.Size(275, 20);
			this.questNameTextBox.TabIndex = 5;
			this.questNameTextBox.TextChanged += new System.EventHandler(this.questNameTextBox_TextChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(34, 133);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Activate:";
			// 
			// activateCheckBox
			// 
			this.activateCheckBox.AutoSize = true;
			this.activateCheckBox.Location = new System.Drawing.Point(146, 133);
			this.activateCheckBox.Name = "activateCheckBox";
			this.activateCheckBox.Size = new System.Drawing.Size(15, 14);
			this.activateCheckBox.TabIndex = 7;
			this.activateCheckBox.UseVisualStyleBackColor = true;
			this.activateCheckBox.CheckedChanged += new System.EventHandler(this.activateCheckBox_CheckedChanged);
			// 
			// AddNewQuest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(470, 276);
			this.Controls.Add(this.activateCheckBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.questNameTextBox);
			this.Controls.Add(this.sectorsComboBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.cancelBtn);
			this.Controls.Add(this.addBtn);
			this.Controls.Add(this.label1);
			this.Name = "AddNewQuest";
			this.Text = "Add new quest";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button addBtn;
		private System.Windows.Forms.Button cancelBtn;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox sectorsComboBox;
		private System.Windows.Forms.TextBox questNameTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox activateCheckBox;
	}
}