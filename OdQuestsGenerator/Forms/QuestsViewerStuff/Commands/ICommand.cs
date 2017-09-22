using OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.Commands
{
	interface ICommand
	{
		void Do();
		void Undo();
	}

	abstract class Command : ICommand
	{
		protected readonly EditingContext Context;

		public abstract void Do();
		public abstract void Undo();

		protected Command(EditingContext context)
		{
			Context = context;
		}
	}
}
