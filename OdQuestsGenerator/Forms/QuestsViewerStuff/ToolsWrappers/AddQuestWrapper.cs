using System.Collections.Generic;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.Commands;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class AddQuestWrapper : ToolWrapper<PlanarShapeCreationTool, FlowView>
	{
		public AddQuestWrapper(EditingContext context, PlanarShapeCreationTool tool, FlowView view)
			: base(context, tool, view)
		{}

		public override void OnShapesInserted(List<Shape> affectedShapes)
		{
			base.OnShapesInserted(affectedShapes);

			if (affectedShapes.Count == 1 && affectedShapes.First() is Box) {
				var dialog = new AddNewQuest(Context.Flow.Sectors);
				var shape = affectedShapes.First() as Box;
				dialog.FormClosing += (s, e) => AddNewQuest_FormClosing(s as AddNewQuest, shape);
				dialog.Show();
			}
		}

		private void AddNewQuest_FormClosing(AddNewQuest form, Box shape)
		{
			if (form.Accepted) {
				Command command = null;
				var quest = Quest.Default;
				quest.Name = form.QuestName;
				quest.SectorName = form.Sector.Name;

				var addQuestCommand = CommandsCreation.AddQuest(quest, form.Sector, Context, DiagramWrapper, shape);
				if (form.ActivateByDefault) {
					var c = new CompositeCommand(Context);
					c.AddCommand(addQuestCommand);
					c.AddCommand(CommandsCreation.ActivateQuest(quest, Context, DiagramWrapper));
					command = c;
				} else {
					command = addQuestCommand;
				}
				Context.History.Do(command);

				shape.SetCaptionText(0, quest.Name);
			} else {
				DiagramWrapper.RemoveNodeShape(shape);
			}
		}
	}
}
