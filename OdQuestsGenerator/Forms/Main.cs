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

			Clipboard.SetText(resultViewer.Text);
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
			var stateForm = new QuestState(currentQuest, state);
			stateForm.Show(this);
			stateForm.Closed += (_1, _2) => {
				if (stateForm.Action != QuestStateProcessAction.None) {
					switch (stateForm.Action) {
						case QuestStateProcessAction.Add:
							if (currentQuest.States.All(s => s.Name != stateForm.ModifiedState.Name)) {
								currentQuest.States.Add(stateForm.ModifiedState);
							}
							break;
						case QuestStateProcessAction.Remove:
							currentQuest.States.Remove(stateForm.OriginalState);
							break;
						case QuestStateProcessAction.Edit:
							var idx = currentQuest.States.IndexOf(stateForm.OriginalState);
							currentQuest.States[idx] = stateForm.ModifiedState;
							break;
					}

					if (stateForm.IsFinal) { 
						if (
							stateForm.Action == QuestStateProcessAction.Add
							|| stateForm.Action == QuestStateProcessAction.Edit
						) {
							currentQuest.FinalState = stateForm.ModifiedState;
						} else if (stateForm.Action == QuestStateProcessAction.Remove) {
							currentQuest.FinalState = null;
						}
					} else if (!stateForm.IsFinal && currentQuest.FinalState == stateForm.OriginalState) {
						currentQuest.FinalState = null;
					}

					OnQuestChanged();
				}
			};
		}
	}
}
