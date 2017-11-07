using System;
using System.Collections.Generic;
using System.Linq;

namespace OdQuestsGenerator.Commands
{
	class CompositeCommand : Command
	{
		private readonly List<ICommand> commands = new List<ICommand>();

		public CompositeCommand()
			: base(null)
		{}

		public CompositeCommand(IEnumerable<ICommand> commands)
			: this()
		{
			AddCommands(commands);
		}

		public void AddCommand(ICommand command)
		{
			commands.Add(command);

			Done += command.Done;
			Undone = (UndoneHandler)Delegate.Combine(command.Undone, Undone);
		}

		public void AddCommands(IEnumerable<ICommand> commandsToAdd)
		{
			commands.AddRange(commandsToAdd);

			foreach (var cmd in commandsToAdd) {
				Done += cmd.Done;
				Undone = (UndoneHandler)Delegate.Combine(cmd.Undone, Undone);
			}
		}

		public override void Do()
		{
			foreach (var cmd in commands) {
				cmd.Do();
			}
		}

		public override void Undo()
		{
			foreach (var cmd in Enumerable.Reverse(commands)) {
				cmd.Undo();
			}
		}
	}
}
