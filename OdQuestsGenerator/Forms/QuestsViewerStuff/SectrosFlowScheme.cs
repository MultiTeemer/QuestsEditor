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
		const int ShapeSize = 300;

		private readonly DiagramSetController controller;
		private readonly Project project;
		private readonly Display display;

		public SectorsFlowScheme(DiagramSetController controller, Project project, Display display)
		{
			this.controller = controller;
			this.project = project;
			this.display = display;
		}

		public void Display(Graph graph)
		{
			var diagram = new Diagram("Foo");
			diagram.Size = new System.Drawing.Size(5000, 2500);

			InitShapes(graph, diagram);

			display.Diagram = diagram;
		}

		private void InitShapes(Graph graph, Diagram diagram)
		{
			var node2shape = new Dictionary<Node, Shape>();
			var link2arrow = new Dictionary<Link, Shape>();

			var OriginalGraph = new Microsoft.Msagl.Core.Layout.GeometryGraph();
			var LayoutAlgorithmSettings = new Microsoft.Msagl.Layout.Layered.SugiyamaLayoutSettings();

			foreach (var n in graph.Nodes) {
				var shape = (Box)project.ShapeTypes["Box"].CreateInstance();
				shape.SetCaptionText(0, n.Quest.Name);
				node2shape.Add(n, shape);
				diagram.Shapes.Add(shape);

				shape.Width =  ShapeSize;
				shape.Height = ShapeSize / 2;

				var node = new Microsoft.Msagl.Core.Layout.Node(Microsoft.Msagl.Core.Geometry.Curves.CurveFactory.CreateRectangle(ShapeSize, ShapeSize, new Microsoft.Msagl.Core.Geometry.Point()), n); ;

				OriginalGraph.Nodes.Add(node);
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

				var n1 = OriginalGraph.Nodes.FirstOrDefault(n => n.UserData == l.Node1);
				var n2 = OriginalGraph.Nodes.FirstOrDefault(n => n.UserData == l.Node2);

				var e = new Microsoft.Msagl.Core.Layout.Edge(n1, n2);


				OriginalGraph.Edges.Add(e);
			}

			LayoutHelpers.CalculateLayout(OriginalGraph, LayoutAlgorithmSettings, null);

			foreach (var kv in node2shape) {
				var pos = OriginalGraph.Nodes.First(n => n.UserData == kv.Key).Center;
				kv.Value.X = (int)pos.X;
				kv.Value.Y = (int)pos.Y;
			}
		}
	}
}
