using System;
using System.Collections.Generic;

namespace OdQuestsGenerator.Commands
{
	class CommandsHistory
	{
		private readonly List<ICommand> commands = new List<ICommand>();

		public int LastPerformedCommandIdx { get; private set; } = -1;

		public event Action<ICommand, bool> Done;
		public event Action<ICommand> Undone;

		public void Do(ICommand command)
		{
			command.Do();

			if (LastPerformedCommandIdx != commands.Count - 1) {
				var idx = LastPerformedCommandIdx + 1;
				commands.RemoveRange(idx, Math.Min(commands.Count, commands.Count - idx));
			}
			commands.Add(command);

			LastPerformedCommandIdx = commands.Count - 1;

			command.Done?.Invoke(true);
			Done?.Invoke(command, true);
		}

		public void Do()
		{
			if (LastPerformedCommandIdx < commands.Count - 1) {
				var command = commands[LastPerformedCommandIdx + 1];
				command.Do();
				++LastPerformedCommandIdx;
				command.Done?.Invoke(false);
				Done?.Invoke(command, false);
			}
		}
		
		public void Undo()
		{
			if (LastPerformedCommandIdx > -1) {
				var command = commands[LastPerformedCommandIdx];
				command.Undo();
				--LastPerformedCommandIdx;
				command.Undone?.Invoke();
				Undone?.Invoke(command);
			}
		}
	}
}
