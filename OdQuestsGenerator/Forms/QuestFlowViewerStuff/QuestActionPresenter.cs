using System;
using System.Collections.Generic;
using System.Linq;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Forms.QuestFlowViewerStuff
{
	static class QuestActionPresenter
	{
		private delegate string PresentationDelegate(QuestAction action);

		private static readonly Dictionary<QuestActionType, PresentationDelegate> presenters = new Dictionary<QuestActionType, PresentationDelegate>();

		static QuestActionPresenter()
		{
			var methods = typeof(QuestActionPresenter).GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			foreach (var m in methods) {
				var attr = (PresenterAttribute)m.GetCustomAttributes(typeof(PresenterAttribute), false).FirstOrDefault();
				if (attr != null) {
					presenters[attr.Type] = (PresentationDelegate)Delegate.CreateDelegate(typeof(PresentationDelegate), m);
				}
			}
		}

		public static string GetPresentation(QuestAction action) =>
			presenters.ContainsKey(action.Type) ? presenters[action.Type](action) : "<some code>";

		[Presenter(QuestActionType.NotParsed)]
		private static string NotParsedPresentation(QuestAction action) => action.Source;

		[Presenter(QuestActionType.SkipFrame)]
		private static string SkipFramePresentation(QuestAction action) => "Skip frame";

		[Presenter(QuestActionType.WaitSomeTime)]
		private static string WaitSomeTimePresentation(QuestAction action) => $"Wait {action.Properties["time"]} seconds";

		[Presenter(QuestActionType.EndState)]
		private static string EndStatePresentation(QuestAction action) => "End state";

		[AttributeUsage(AttributeTargets.Method)]
		private class PresenterAttribute : Attribute
		{
			public readonly QuestActionType Type;

			public PresenterAttribute(QuestActionType type)
			{
				Type = type;
			}
		}
	}
}
