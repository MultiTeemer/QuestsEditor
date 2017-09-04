using System.Windows.Forms;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Forms
{
	internal enum QuestStateProcessAction
	{
		None,
		Add,
		Edit,
		Remove,
	}

	public partial class QuestState : Form
	{
		public readonly State OriginalState;
		public readonly State ModifiedState;

		private readonly Quest quest;
		private readonly bool createNew;

		public bool IsFinal { get; private set; }

		internal QuestStateProcessAction Action { get; private set; }

		public QuestState(Quest quest, State state = null)
		{
			this.quest = quest;

			createNew = state == null;
			OriginalState = state ?? State.Dumb;
			Action = QuestStateProcessAction.None;
			ModifiedState = OriginalState.Clone();

			InitializeComponent();

			Customize();
		}

		private void Customize()
		{
			if (Action == QuestStateProcessAction.Add) {
				saveButton.Text = "Create";
			}

			nameTextBox.Text = OriginalState.Name;
			isFinalCheckBox.Checked = quest.FinalState == OriginalState;
		}

		private void nameTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			ModifiedState.Name = nameTextBox.Text;
			saveButton.Enabled = IsItOkForCodeGeneration.Check(ModifiedState.Name);

			if (e.KeyCode == Keys.Enter && saveButton.Enabled) {
				saveButton_Click(sender, e);
			}
		}

		private void saveButton_Click(object sender, System.EventArgs e)
		{
			Action = createNew ? QuestStateProcessAction.Add : QuestStateProcessAction.Edit;

			Close();
		}

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			Action = QuestStateProcessAction.Remove;

			Close();
		}

		private void isFinalCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			IsFinal = isFinalCheckBox.Checked;
		}
	}
}
