using System;
using System.Collections.Generic;
using System.Linq;
using Dataweb.NShape;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.GeneralShapes;
using Dataweb.NShape.WinFormsUI;
using Microsoft.Msagl.Miscellaneous;
using OdQuestsGenerator.Data;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff
{
	class SectorsFlowScheme
	{
		const int ShapeSize = 200;

		private readonly DiagramSetController controller;
		private readonly Project project;
		private readonly Display display;

		private Dictionary<Node, Shape> node2shape = new Dictionary<Node, Shape>();
		private Dictionary<Link, Shape> link2arrow = new Dictionary<Link, Shape>();

		private Diagram diagram;

		public SectorsFlowScheme(DiagramSetController controller, Project project, Display display)
		{
			this.controller = controller;
			this.project = project;
			this.display = display;
		}

		public void Display(Graph graph)
		{
			diagram = new Diagram("Foo");
			diagram.Size = new System.Drawing.Size(3000, 1500);

			InitShapes(graph, diagram);
			LayoutShapes(graph);

			display.Diagram = diagram;
		}

		private void InitShapes(Graph graph, Diagram diagram)
		{
			foreach (var n in graph.Nodes) {
				var shape = (Box)project.ShapeTypes["Box"].CreateInstance();
				shape.SetCaptionText(0, n.Quest.Name);
				node2shape.Add(n, shape);
				diagram.Shapes.Add(shape);

				shape.Width =  ShapeSize;
				shape.Height = ShapeSize / 2;
			}

			foreach (var l in graph.Links) {
				var arrow = (Polyline)project.ShapeTypes["Polyline"].CreateInstance();

				var shape1 = node2shape[l.Node1];
				var shape2 = node2shape[l.Node2];

				arrow.EndCapStyle = project.Design.CapStyles.ClosedArrow;
				arrow.Connect(ControlPointId.FirstVertex, shape1, ControlPointId.Reference);
				arrow.Connect(ControlPointId.LastVertex, shape2, ControlPointId.Reference);

				link2arrow.Add(l, arrow);
				diagram.Shapes.Add(arrow);
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

			Func<Node, Microsoft.Msagl.Core.Layout.Node> ensureNode = node => {
				var res = geomGraph.Nodes.FirstOrDefault(n => n.UserData == node);
				if (res == null) {
					var _node = new Microsoft.Msagl.Core.Layout.Node(
						Microsoft.Msagl.Core.Geometry.Curves.CurveFactory.CreateRectangle(
							ShapeSize,
							ShapeSize,
							new Microsoft.Msagl.Core.Geometry.Point()
						),
						node
					);

					geomGraph.Nodes.Add(_node);
					return _node;
				}
				else {
					return res;
				}
			};

			foreach (var l in graph.Links) {
				var n1 = ensureNode(l.Node1);
				var n2 = ensureNode(l.Node2);
				var e = new Microsoft.Msagl.Core.Layout.Edge(n1, n2);
				geomGraph.Edges.Add(e);
			}

			LayoutHelpers.CalculateLayout(geomGraph, layoutSettings, null);

			foreach (var kv in node2shape) {
				var pos = geomGraph.Nodes.FirstOrDefault(n => n.UserData == kv.Key)?.Center;
				if (pos.HasValue) {
					kv.Value.X = (int)pos.Value.X + ShapeSize;
					kv.Value.Y = diagram.Height - (int)pos.Value.Y + ShapeSize / 2;
				}
			}

			geometry = geomGraph;
		}

		private void LayoutFreeShapes(Microsoft.Msagl.Core.Layout.GeometryGraph geometry)
		{
			var xs = geometry.Nodes.Select(n => n.Center.X);
			var right = xs.Max() + ShapeSize * 3;
			var freeShapes = node2shape.Where(kv => geometry.Nodes.All(n => n.UserData != kv.Key)).Select(kv => kv.Value).ToArray();
			var width = diagram.Size.Width - right;
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
