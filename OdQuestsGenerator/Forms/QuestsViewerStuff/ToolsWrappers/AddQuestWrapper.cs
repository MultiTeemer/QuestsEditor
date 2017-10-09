using System.Collections.Generic;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.GeneralShapes;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	class AddQuestWrapper : ToolWrapper<PlanarShapeCreationTool>
	{
		public AddQuestWrapper(EditingContext context, PlanarShapeCreationTool tool)
			: base(context, tool)
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

		private void AddNewQuest_FormClosing(AddNewQuest d, Box shape)
		{
			if (d.Accepted) {
				Command command = null;
				var quest = Quest.Default;
				quest.Name = d.QuestName;
				quest.SectorName = d.Sector.Name;

				if (d.ActivateByDefault) {
					var c = new CompositeCommand(Context);
					c.AddCommand(new AddQuestCommand(quest, d.Sector, shape, Context));
					c.AddCommand(new ActivateQuestCommand(quest, d.Sector, Context));
					command = c;
				} else {
					command = new AddQuestCommand(quest, d.Sector, shape, Context);
				}
				Context.History.Do(command);

				shape.SetCaptionText(0, quest.Name);
			} else {
				Context.FlowView.RemoveNodeShape(shape);
			}
		}
	}
}
