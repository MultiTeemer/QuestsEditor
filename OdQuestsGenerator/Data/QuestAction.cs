using System;
using System.Collections.Generic;

namespace OdQuestsGenerator.Data
{
	enum QuestActionType
	{
		NotParsed,
		SkipFrame,
		WaitSomeTime,
		EndState,
	}

	class QuestAction
	{
		public readonly string Id = Guid.NewGuid().ToString();

		public QuestActionType Type { get; set; } = QuestActionType.NotParsed;

		public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

		public string Source { get; set; }

		public override string ToString() => Source;
	}

	class QuestActionsData : IData
	{
		public List<QuestAction> Actions = new List<QuestAction>();
	}
}
