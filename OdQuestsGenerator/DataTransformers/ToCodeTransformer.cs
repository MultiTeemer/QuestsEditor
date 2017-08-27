using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers.Templates;

namespace OdQuestsGenerator.DataTransformers
{
	static class ToCodeTransformer
	{
		public static string Transform(State state, Quest quest)
		{
			var t = new StateTaskTemplate {
				StateEnumName = quest.Name + "QuestState",
				StateName = state.Name,
			};
			return t.TransformText();
		}

		public static string Transform(Quest quest)
		{
			var t = new QuestClassTemplate {
				Quest = quest,
			};
			return t.TransformText();
		}
	}
}
