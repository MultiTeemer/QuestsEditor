namespace OdQuestsGenerator.Data
{
	public class State
	{
		public string Name { get; set; }

		public static State Dumb => new State {
			Name = "Initial",
		};

		public State Clone()
		{
			return MemberwiseClone() as State;
		}
	}
}
