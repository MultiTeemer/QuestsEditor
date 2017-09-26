using System.Collections.Generic;
using System.Linq;
using OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Commands
{
	class CompositeCommand : Command
	{
		private readonly List<ICommand> commands = new List<ICommand>();

		public CompositeCommand(EditingContext context)
			: base(context)
		{}

		public CompositeCommand(EditingContext context, IEnumerable<ICommand> commands)
			: this(context)
		{
			AddCommands(commands);
		}

		public void AddCommand(ICommand command)
		{
			commands.Add(command);
		}

		public void AddCommands(IEnumerable<ICommand> commands)
		{
			this.commands.AddRange(commands);
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
