using System;
using System.Linq;
using System.Windows.Forms;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;

namespace OdQuestsGenerator.Forms
{
	public partial class Main : Form
	{
		private readonly Quest currentQuest = Quest.Default;

		public Main()
		{
			InitializeComponent();
			OnQuestChanged();
		}

		private void changeNameBtn_Click(object sender, EventArgs e)
		{
			var changeNameForm = new EnterNewQuestName(currentQuest.Name);
			changeNameForm.Show(this);
			changeNameForm.Closed += (_1, _2) => {
				if (changeNameForm.Accepted) {
					currentQuest.Name = changeNameForm.NewName;

					OnQuestChanged();
				}
			};
		}

		private void OnQuestChanged()
		{
			resultViewer.Text = ToCodeTransformer.Transform(currentQuest);

			statesViewer.Items.Clear();
			statesViewer.Items.AddRange(currentQuest.States.Select(s => s.Name).ToArray());
		}

		private void addStateButton_Click(object sender, EventArgs e)
		{
			ShowStateForm();
		}

		private void statesViewer_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int index = statesViewer.IndexFromPoint(e.Location);
			if (index != ListBox.NoMatches) {
				ShowStateForm(currentQuest.States[index]);
			}
		}

		private void ShowStateForm(State state = null)
		{
			var stateForm = new QuestState(state);
			stateForm.Show(this);
			stateForm.Closed += (_1, _2) => {





				if (stateForm.Accepted) {
					if (stateForm.CreateNew) {
						currentQuest.States.Add(stateForm.State);
					}

					OnQuestChanged();
				}
			};
		}
	}
}
