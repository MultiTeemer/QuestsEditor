using OdQuestsGenerator.Data;
using OdQuestsGenerator.DataTransformers.Templates;

namespace OdQuestsGenerator.DataTransformers
{
	static class ToCodeTransformer
	{
		public static string Transform(Quest quest)
		{
			var t = new QuestClassTemplate {
				Quest = quest,
			};
			return t.TransformText();
		}
	}
}
