using System.Collections.Generic;

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
	}
}
