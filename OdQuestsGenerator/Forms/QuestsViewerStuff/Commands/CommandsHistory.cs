using System.Collections.Generic;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Commands
{
	class CommandsHistory
	{
		private readonly List<ICommand> commands = new List<ICommand>();

		private int lastPerformedCommandIdx = -1;

		public void Do(ICommand command)
		{
			command.Do();

			if (lastPerformedCommandIdx != commands.Count - 1) {
				commands.RemoveRange(lastPerformedCommandIdx + 1, commands.Count);
			}
			commands.Add(command);
		}

		public void Do()
		{
			if (lastPerformedCommandIdx < commands.Count - 1) {
				commands[lastPerformedCommandIdx + 1].Do();
				++lastPerformedCommandIdx;
			}
		}
		
		public void Undo()
		{
			if (lastPerformedCommandIdx > -1) {
				commands[lastPerformedCommandIdx].Undo();
				--lastPerformedCommandIdx;
			}
		}
	}
}
