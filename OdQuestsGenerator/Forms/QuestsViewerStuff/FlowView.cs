using System;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.WinFormsUI;
using Microsoft.Msagl.Miscellaneous;
using OdQuestsGenerator.CodeReaders;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing;
using OdQuestsGenerator.Utils;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
{
	class FlowView : DiagramWrapper
	{
		public const int ShapeSize = 200;

		private readonly ShapeTemplatesFactory templates;
		private readonly PresentersManager presentersManager;

		private readonly TwoWayDictionary<Node, Shape> nodesAndShapes = new TwoWayDictionary<Node, Shape>();
		private readonly TwoWayDictionary<Link, Shape> linksAndArrows = new TwoWayDictionary<Link, Shape>();

		public FlowView(
			DiagramSetController controller,
			Project project,
			Display display,
			ShapeTemplatesFactory templates
		)
			: base(controller, project, display)
		{
			this.templates = templates;

			presentersManager = new PresentersManager(project);
		}

		public void DisplayFlow(Graph graph)
		{
			if (Diagram != null) {
				Clear();
			}

			InitDiagram(new System.Drawing.Size(3000, 1500));
			InitShapes(graph, Diagram);
			LayoutShapes(graph);
		}

		public void Update()
		{
			foreach (var kv in nodesAndShapes) {
				presentersManager.GetPresenterFor(kv.Key).Apply(kv.Value);
			}
			foreach (var kv in linksAndArrows) {
				presentersManager.GetPresenterFor(kv.Key).Apply(kv.Value);
			}
		}

		public Node GetNodeForShape(Shape shape) => nodesAndShapes.Contains(shape) ? nodesAndShapes[shape] : null;

		public Link? GetLinkForShape(Shape shape) => linksAndArrows.Contains(shape) ? linksAndArrows[shape] : (Link?)null;

		public CaptionedShapeBase GetShapeForQuest(Quest quest)
		{
			var key = nodesAndShapes.Values1.FirstOrDefault(n => n.Quest == quest);
			return key != null ? nodesAndShapes[key] as CaptionedShapeBase : null;
		}

		public void AddShapeForNode(Node node, Shape shape)
		{
			nodesAndShapes[node] = shape;
			presentersManager.GetPresenterFor(node).Apply(shape);
			AddShape(shape);
		}

		public void RegisterShapeForNode(Node node, Shape shape)
		{
			nodesAndShapes[node] = shape;
			presentersManager.GetPresenterFor(node).Apply(shape);
		}

		public void RemoveNodeShape(Shape shape)
		{
			nodesAndShapes.Remove(shape);
			RemoveShape(shape);
		}

		public void RemoveShapeLink(Link link)
		{
			if (linksAndArrows.Contains(link)) {
				RemoveShape(linksAndArrows[link]);
				linksAndArrows.Remove(link);
			}
		}

		public void AddShapeLink(Link link)
		{
			if (!linksAndArrows.Contains(link)) {
				var arrow = templates.GetLinkTemplate();
				var shape1 = nodesAndShapes[link.Node1];
				var shape2 = nodesAndShapes[link.Node2];
				arrow.Connect(ControlPointId.FirstVertex, shape1, ControlPointId.Reference);
				arrow.Connect(ControlPointId.LastVertex, shape2, ControlPointId.Reference);

				linksAndArrows.Add(link, arrow);
				presentersManager.GetPresenterFor(link).Apply(arrow);
				AddShape(arrow);
			}
		}

		public void RegisterShapeForLink(Shape shape, Link link)
		{
			linksAndArrows.Add(link, shape);
			presentersManager.GetPresenterFor(link).Apply(shape);
		}

		public override void Clear()
		{
			linksAndArrows.Clear();
			nodesAndShapes.Clear();

			ClearDiagram();
		}

		private void InitShapes(Graph graph, Diagram diagram)
		{
			foreach (var n in graph.Nodes) {
				AddShapeForNode(n, templates.GetQuestTemplate(n.Quest.Name));
			}

			foreach (var l in graph.Links) {
				AddShapeLink(l);
			}
		}

		private void LayoutShapes(Graph graph)
		{
			LayoutLinkedShapes(graph, out var geometry);
			LayoutFreeShapes(geometry);
		}

		private void LayoutLinkedShapes(Graph graph, out Microsoft.Msagl.Core.Layout.GeometryGraph geometry)
		{
			var geomGraph = new Microsoft.Msagl.Core.Layout.GeometryGraph();
			var layoutSettings = new Microsoft.Msagl.Layout.Layered.SugiyamaLayoutSettings();

			Microsoft.Msagl.Core.Layout.Node EnsureNode(Node node)
			{
				var res = geomGraph.Nodes.FirstOrDefault(n => n.UserData == node);
				if (res == null) {
					var geomNode = new Microsoft.Msagl.Core.Layout.Node(
						Microsoft.Msagl.Core.Geometry.Curves.CurveFactory.CreateRectangle(
							ShapeSize,
							ShapeSize,
							new Microsoft.Msagl.Core.Geometry.Point()
						),
						node
					);

					geomGraph.Nodes.Add(geomNode);

					return geomNode;
				}

				return res;
			}

			foreach (var l in graph.Links) {
				var n1 = EnsureNode(l.Node1);
				var n2 = EnsureNode(l.Node2);
				var e = new Microsoft.Msagl.Core.Layout.Edge(n1, n2);
				geomGraph.Edges.Add(e);
			}

			LayoutHelpers.CalculateLayout(geomGraph, layoutSettings, null);

			foreach (var kv in nodesAndShapes) {
				var pos = geomGraph.Nodes.FirstOrDefault(n => n.UserData == kv.Key)?.Center;
				if (pos.HasValue) {
					kv.Value.X = (int)pos.Value.X + ShapeSize;
					kv.Value.Y = Diagram.Height - (int)pos.Value.Y + ShapeSize / 2;
				}
			}

			geometry = geomGraph;
		}

		private void LayoutFreeShapes(Microsoft.Msagl.Core.Layout.GeometryGraph geometry)
		{
			var xs = geometry.Nodes.Select(n => n.Center.X);
			var right = xs.Any() ? xs.Max() + ShapeSize * 3 : ShapeSize;
			var freeShapes = nodesAndShapes.Where(kv => geometry.Nodes.All(n => n.UserData != kv.Key)).Select(kv => kv.Value).ToArray();
			var priorities = freeShapes.Select(s => {
				var q = nodesAndShapes[s].Quest;
				if (!q.IsLinksToEditable()) {
					return 3;
				}

				if (!q.IsActive()) {
					return 2;
				}

				return 1;
			}).ToArray();
			Array.Sort(priorities, freeShapes);
			var width = Diagram.Size.Width - right;
			var columnsCount = Math.Max(1, width / ShapeSize - 1);
			int curColumn = 0;
			int curRow = 0;
			foreach (var s in freeShapes) {
				s.X = curColumn * ShapeSize + (int)right;
				s.Y = curRow * ShapeSize + ShapeSize / 2;

				++curColumn;
				if (curColumn >= columnsCount) {
					curColumn = 0;
					++curRow;
				}
			}
		}
	}
}
