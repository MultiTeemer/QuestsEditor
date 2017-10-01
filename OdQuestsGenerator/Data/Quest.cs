using System.Collections.Generic;
using System.Linq;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Data
{
	public interface IData {}

	public class Quest
	{
		public string Name { get; set; }
		public string SectorName { get; set; }
		public List<State> States { get; set; }
		public State FinalState { get; set; }
		public List<IData> Data { get; set; } = new List<IData>();

		public static Quest Default
		{
			get
			{
				var q = new Quest {
					Name = "Default",
					States = new List<State> {
						new State {
							Name = "Initial",
						},
						new State {
							Name = "Final",
						},
					}
				};
				q.FinalState = q.States.Last();
				return q;
			}
		}

		public override string ToString() => Name;

		public TData Ensure<TData>()
			where TData : IData, new()
		{
			var data = Data.FirstOrDefault(d => d is TData);
			if (data == null) {
				Data.Add(new TData());
				return (TData)Data.Last();
			}
			return (TData)data;
		}
	}
}
