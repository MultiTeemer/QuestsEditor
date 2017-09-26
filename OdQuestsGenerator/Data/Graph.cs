using System.Collections.Generic;
using System.Linq;

namespace OdQuestsGenerator.Data
{
	class Node
	{
		public Quest Quest { get; set; }
	}

	struct Link
	{
		public readonly Node Node1;
		public readonly Node Node2;

		public Link(Node node1, Node node2)
		{
			Node1 = node1;
			Node2 = node2;
		}

		public bool Contains(Node node)
		{
			return Node1 == node || Node2 == node;
		}
	}

	class Graph
	{
		private readonly List<Node> nodes = new List<Node>();
		private readonly List<Link> links = new List<Link>();

		public IReadOnlyList<Node> Nodes => nodes;
		public IReadOnlyList<Link> Links => links;

		public void AddNode(Node node)
		{
			if (!nodes.Contains(node)) {
				nodes.Add(node);
			}
		}

		public void AddNode(Quest quest)
		{
			if (FindNodeForQuest(quest) == null) {
				nodes.Add(new Node() { Quest = quest });
			}
		}

		public void RemoveNode(Node node)
		{
			nodes.Remove(node);
			links.RemoveAll(l => l.Contains(node));
		}

		public void RemoveNode(Quest quest)
		{
			RemoveNode(FindNodeForQuest(quest));
		}

		public void AddLink(Node n1, Node n2, bool bidirectional = false)
		{
			if (!ExistsLink(n1, n2))
			links.Add(new Link(n1, n2));
			if (bidirectional && !ExistsLink(n2, n1)) {
				links.Add(new Link(n2, n1));
			}
		}

		public void AddLink(Quest quest1, Quest quest2)
		{
			var n1 = FindNodeForQuest(quest1);
			var n2 = FindNodeForQuest(quest2);

			if (n1 != null && n2 != null && !ExistsLink(n1, n2)) {
				links.Add(new Link(n1, n2));
			}
		}

		public void AddLink(Link link)
		{
			if (!ExistsLink(link.Node1, link.Node2)) {
				links.Add(link);
			}
		}

		public Node FindNodeForQuest(Quest quest)
		{
			return nodes.FirstOrDefault(n => n.Quest == quest);
		}

		public Node FindNodeForQuest(string questName)
		{
			return nodes.FirstOrDefault(n => n.Quest.Name == questName);
		}

		public bool ExistsLink(Node node1, Node node2)
		{
			return links.Any(l => l.Node1 == node1 && l.Node2 == node2);
		}

		public IEnumerable<Link> GetLinksForNode(Node node)
		{
			return links.Where(l => l.Contains(node));
		}
	}
}
