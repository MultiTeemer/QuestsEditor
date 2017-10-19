using System.Collections.Generic;
using System.Linq;

namespace OdQuestsGenerator.Data
{
	public class State
	{
		public string Name { get; set; }
		public List<IData> Data { get; set; } = new List<IData>();

		public static State Dumb => new State {
			Name = "Initial",
		};

		public State Clone() => MemberwiseClone() as State;

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
