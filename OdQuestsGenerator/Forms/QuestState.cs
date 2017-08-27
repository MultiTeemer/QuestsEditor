using System.Windows.Forms;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Forms
{
	internal enum QuestStateProcessAction
	{
		Add,
		Edit,
		Remove,
	}

	public partial class QuestState : Form
	{
		public readonly State State;

		internal QuestStateProcessAction Action { get; private set; }

		public QuestState(State state = null)
		{
			State = state ?? State.Dumb;
			Action = state == null ? QuestStateProcessAction.Add : QuestStateProcessAction.Edit;

			InitializeComponent();

			Customize();
		}

		private void Customize()
		{
			if (Action == QuestStateProcessAction.Add) {
				saveButton.Text = "Create";
			}

			nameTextBox.Text = State.Name;
		}

		private void nameTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			State.Name = nameTextBox.Text;
			saveButton.Enabled = IsItOkForCodeGeneration.Check(State.Name);
		}

		private void saveButton_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			Action = QuestStateProcessAction.Remove;

			Close();
		}
	}
}
