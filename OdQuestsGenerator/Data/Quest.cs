using System.Collections.Generic;

namespace OdQuestsGenerator.Data
{
	class Quest
	{
		public string Name { get; set; }
		public List<State> States { get; set; }
		public State FinalState { get; set; }

		public static Quest Default => new Quest {
			Name = "Default",
			States = new List<State>(),
		};
	}
}
