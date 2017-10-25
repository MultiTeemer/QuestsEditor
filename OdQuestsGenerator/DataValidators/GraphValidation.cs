using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.DataValidators
{
	static class GraphValidation
	{
		public static (bool loopForms, List<Node> loop) LoopForms(Graph graph, Link newEdge) =>
			CanReach(graph, newEdge.Node2, newEdge.Node1);

		private static (bool result, List<Node> path) CanReach(Graph graph, Node start, Node required) =>
			CanReach(graph, start, new List<Node>(), required);

		private static (bool result, List<Node> path) CanReach(Graph graph, Node current, List<Node> path, Node required)
		{
			var links = graph.GetOutcomingLinksForNode(current).ToArray();
			path.Add(current);

			if (links.Any(l => l.Node2 == required)) {
				path.Add(required);
				return (true, path);
			}

			foreach (var l in links) {
				var res = CanReach(graph, l.Node2, path, required);
				if (res.result) {
					return res;
				}
			}

			path.RemoveAt(path.Count - 1);

			return (false, null);
		}
	}
}
