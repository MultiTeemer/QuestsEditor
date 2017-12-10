using System.Collections.Generic;
using System.Linq;

namespace OdQuestsGenerator.Data
{
	public interface IData {}

	public class DataStorage
	{
		public List<IData> Records = new List<IData>();

		public TData Ensure<TData>()
			where TData : IData, new()
		{
			var data = Records.FirstOrDefault(d => d is TData);
			if (data == null) {
				Records.Add(new TData());
				return (TData)Records.Last();
			}
			return (TData)data;
		}
	}

	public class Quest
	{
		public string Name { get; set; }
		public string SectorName { get; set; }
		public List<State> States { get; set; }
		public State FinalState { get; set; }
		public DataStorage Data { get; set; } = new DataStorage();

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
	}
}
