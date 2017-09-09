using System.Collections.Generic;
using System.Linq;

namespace OdQuestsGenerator.Data
{
	public class Quest
	{
		public string Name { get; set; }
		public string SectorName { get; set; }
		public List<State> States { get; set; }
		public State FinalState { get; set; }

		public static Quest Default
		{
			get
			{
				var q = new Quest {
					Name = "Default",
					States = new List<State> {
						new State {
							Name = "Initial",
						},
						new State {
							Name = "Final",
						},
					}
				};
				q.FinalState = q.States.Last();
				return q;
			}
		}
	}
}
