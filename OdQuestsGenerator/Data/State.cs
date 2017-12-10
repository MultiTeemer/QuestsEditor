using System.Collections.Generic;
using System.Linq;

namespace OdQuestsGenerator.Data
{
	public class State
	{
		public string Name { get; set; }
		public DataStorage Data { get; set; } = new DataStorage();

		public static State Dumb => new State {
			Name = "Initial",
		};

		public State Clone() => MemberwiseClone() as State;

		public override string ToString() => Name;
	}
}
