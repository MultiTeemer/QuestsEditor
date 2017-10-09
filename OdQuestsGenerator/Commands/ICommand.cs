namespace OdQuestsGenerator.Commands
{
	delegate void DoneHandler(bool firstTime);
	delegate void UndoneHandler();

	interface ICommand
	{
		DoneHandler Done { get; set; }
		UndoneHandler Undone { get; set; }

		void Do();
		void Undo();
	}

	abstract class Command : ICommand
	{
		protected readonly EditingContext Context;

		public DoneHandler Done { get; set; }
		public UndoneHandler Undone { get; set; }

		public abstract void Do();
		public abstract void Undo();

		protected Command(EditingContext context)
		{
			Context = context;
		}
	}
}
