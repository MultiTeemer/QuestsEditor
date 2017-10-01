using System.Collections.Generic;

namespace OdQuestsGenerator.Data
{
	class Sector
	{
		public string Name { get; set; }
		public List<Quest> Quests { get; set; }

		public override string ToString() => Name;
	}
}
