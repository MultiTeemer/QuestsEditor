using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms
{
	public partial class GenerateQuestStub : Form
	{
		private const float DeltaPosToStartDragging = 0.5f;

		private readonly Quest currentQuest = Quest.Default;

		private int? draggingStateIndex;
		private int? currentDraggingStateIndex;
		private Point? mousePressLocation;

		public GenerateQuestStub()
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
			GenerateCode();
			FillStatesViewer();
		}

		private void GenerateCode()
		{
			var generatedCode = ToCodeTransformer.Transform(currentQuest);
			resultViewer.Text = generatedCode;
			Clipboard.SetText(generatedCode);
		}

		private void FillStatesViewer()
		{
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

		private void statesViewer_DragDrop(object sender, DragEventArgs e)
		{
			Point point = statesViewer.PointToClient(new Point(e.X, e.Y));
			int destIndex = statesViewer.IndexFromPoint(point);
			if (destIndex < 0) {
				destIndex = statesViewer.Items.Count - 1;
			}

			if (draggingStateIndex.HasValue) {
				currentQuest.States.Move(draggingStateIndex.Value, destIndex);

				draggingStateIndex = null;

				OnQuestChanged();
			}
		}

		private void statesViewer_MouseDown(object sender, MouseEventArgs e)
		{
			mousePressLocation = e.Location;
		}

		private void statesViewer_DragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Move;

			var point = statesViewer.PointToClient(new Point(e.X, e.Y));
			var destIndex = statesViewer.IndexFromPoint(point);
			if (destIndex < 0) {
				destIndex = statesViewer.Items.Count - 1;
			}

			if (currentDraggingStateIndex != destIndex) {
				FillStatesViewer();

				statesViewer.Items.Move(draggingStateIndex.Value, destIndex);

				currentDraggingStateIndex = destIndex;

				statesViewer.Invalidate();
			}
		}

		private void statesViewer_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && statesViewer.SelectedIndex != -1) {
				currentQuest.States.RemoveAt(statesViewer.SelectedIndex);
				OnQuestChanged();
			}
		}

		private void statesViewer_MouseMove(object sender, MouseEventArgs e)
		{
			if (mousePressLocation.HasValue) {
				Func<double, double> sqr = s => Math.Pow(s, 2);
				var dpos = Math.Sqrt(
					sqr(mousePressLocation.Value.X - e.Location.X) + sqr(mousePressLocation.Value.Y - e.Location.Y)
				);
				if (dpos > DeltaPosToStartDragging) {
					int index = statesViewer.IndexFromPoint(mousePressLocation.Value);
					if (index != -1) {
						draggingStateIndex = index;
						currentDraggingStateIndex = index;
						var s = statesViewer.Items[index].ToString();
						DragDropEffects effects = DoDragDrop(s, DragDropEffects.Move);
						statesViewer.DoDragDrop(sender, effects);
					}
				}
				mousePressLocation = null;
			}
		}
	}
}
