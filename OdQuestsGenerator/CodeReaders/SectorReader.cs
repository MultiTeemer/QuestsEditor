using System.Collections.Generic;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;

namespace OdQuestsGenerator.CodeReaders
{
	class SectorReader : CodeReader
	{
		public override CodeBulkType[] AcceptedTypes => new[] { CodeBulkType.Sector };

		public override void Read(CodeBulk codeBulk, Code code, ref Flow flow)
		{
			var sectorName = FromCodeTransformer.FetchSectorName(codeBulk.Tree);
			var sector = new Sector {
				Name = sectorName,
				Quests = new List<Quest>(),
			};

			flow.Sectors.Add(sector);

			code.SectorsAndCodeBulks[sector] = codeBulk;
		}
	}
}
