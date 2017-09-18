using System;
using System.Linq;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers;
using OdQuestsGenerator.Forms.QuestsViewerStuff;

namespace OdQuestsGenerator.CodeReaders
{
	class QuestReader : CodeReader
	{
		public override CodeBulkType[] AcceptedTypes => new[] { CodeBulkType.Quest,  };

		public override void Read(CodeBulk codeBulk, Code code, ref Flow flow)
		{
			var quest = FromCodeTransformer.ReadQuest(codeBulk.Tree);
			var sector = flow.Sectors.FirstOrDefault(s => s.Name == quest.SectorName);
			if (sector == null) {
				throw new Exception($"Couldn't find sector by name {quest.SectorName}");
			}

			sector.Quests.Add(quest);
			flow.Graph.AddNode(quest);
			code.RegisterQuestforCodeBulk(quest, codeBulk);
		}
	}
}
