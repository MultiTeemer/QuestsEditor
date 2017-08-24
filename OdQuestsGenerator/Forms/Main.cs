using System;
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
		}
	}
}
