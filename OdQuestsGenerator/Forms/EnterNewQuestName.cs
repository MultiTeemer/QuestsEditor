using System.Windows.Forms;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Forms
{
	public partial class EnterNewQuestName : Form
	{
		public bool Accepted { get; private set; }
		public string NewName { get; private set; }

		public EnterNewQuestName(string currentName)
		{
			InitializeComponent();

			questNameTextBox.Text = currentName;
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			Accepted = true;

			Close();
		}

		private void questNameTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			var isValid = IsItOkForCodeGeneration.Check(questNameTextBox.Text);
			okButton.Enabled = isValid;
			if (isValid) {
				NewName = questNameTextBox.Text;
			}

			if (e.KeyCode == Keys.Enter && isValid) {
				okButton_Click(sender, e);
			}
		}

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			Accepted = false;

			Close();
		}
	}
}
