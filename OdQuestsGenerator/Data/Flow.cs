using System.Collections.Generic;
using System.Linq;

namespace OdQuestsGenerator.Data
{
	struct Flow
	{
		public readonly Graph Graph;
		public readonly List<Sector> Sectors;

		public Flow(Graph graph, List<Sector> sectors)
		{
			Graph = graph;
			Sectors = sectors;
		}

		public Sector GetSectorForQuest(Quest quest) => Sectors.First(s => s.Quests.Contains(quest));
	}
}
