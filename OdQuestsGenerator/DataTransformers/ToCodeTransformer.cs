using System;
using System.IO;
using System.Linq;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.DataTransformers
{
	static class ToCodeTransformer
	{
		public static string Transform(State state, Quest quest)
		{
			return ReadTemplate("DataTransformers/Templates/QuestMethod.txt")
				.Replace(AsKey("stateName"), state.Name)
				.Replace("enumName", AsKey($"{quest.Name}QuestState"))
			;
		}

		public static string Transform(Quest quest)
		{
			return ReadTemplate("DataTransformers/Templates/QuestClass.txt")
				.Replace(AsKey("questName"), quest.Name)
				.Replace(AsKey("states"), FormatStates(quest))
				.Replace(AsKey("statesMethods"), FormateStatesMethods(quest))
			;
		}

		private static string AsKey(string key)
		{
			return $"{{{key}}}";
		}

		private static string Replace(string data, string key, string value)
		{
			return data.Replace($"{{{key}}}", value);
		}

		private static string ReadTemplate(string path)
		{
			var template = "";
			using (var reader = new StreamReader(File.Open(path, FileMode.Open))) {
				template = reader.ReadToEnd();
			}

			return template;
		}

		private static string FormatStates(Quest quest)
		{
			return String.Join($",{Environment.NewLine}", quest.States.Select(s => s.Name));
		}

		private static string FormateStatesMethods(Quest quest)
		{
			return String.Join(
				$"{Environment.NewLine}{Environment.NewLine}",
				quest.States.Select(state => Transform(state, quest))
			);
		}
	}
}
