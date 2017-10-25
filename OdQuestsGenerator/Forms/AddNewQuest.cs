using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataValidators;

namespace OdQuestsGenerator.Forms
{
	public partial class AddNewQuest : Form
	{
		private const string DefaultQuestName = "Default";

		private readonly List<Sector> sectors;

		public string QuestName { get; private set; }
		public bool Accepted { get; private set; }
		public bool ActivateByDefault { get; private set; }

		internal Sector Sector { get; private set; }

		internal AddNewQuest(List<Sector> sectors)
		{
			this.sectors = sectors;

			InitializeComponent();
			Customize();
		}

		private void Customize()
		{
			questNameTextBox.Text = DefaultQuestName;

			var sectorsToItems = sectors.Select(s => $"{s.Name} ({s.Quests.Count})");
			sectorsComboBox.Items.AddRange(sectorsToItems.ToArray());

			sectorsComboBox.SelectedIndex = 0;
		}

		private void questNameTextBox_TextChanged(object sender, System.EventArgs e)
		{
			QuestName = questNameTextBox.Text;

			UpdateAddBtn();
		}

		private void sectorsComboBox_SelectedValueChanged(object sender, System.EventArgs e)
		{
			Sector = sectors[sectorsComboBox.SelectedIndex];

			UpdateAddBtn();
		}

		private void UpdateAddBtn()
		{
			addBtn.Enabled = sectorsComboBox.SelectedIndex != -1
				&& IsQuestNameAcceptible(questNameTextBox.Text, sectors[sectorsComboBox.SelectedIndex]);
		}

		private bool IsQuestNameAcceptible(string questName, Sector sector) =>
			IsItOkForCodeGeneration.Check(questName) && sector.Quests.All(q => q.Name != questName);

		private void addBtn_Click(object sender, System.EventArgs e)
		{
			Accepted = true;

			Close();
		}

		private void cancelBtn_Click(object sender, System.EventArgs e)
		{
			Accepted = false;

			Close();
		}

		private void activateCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			ActivateByDefault = activateCheckBox.Checked;
		}
	}
}
