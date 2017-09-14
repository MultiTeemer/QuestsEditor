using System.Collections.Generic;
using Dataweb.NShape;
using Dataweb.NShape.Controllers;
using Dataweb.NShape.GeneralShapes;
using Dataweb.NShape.Layouters;
using Dataweb.NShape.WinFormsUI;
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
			Layout(diagram, graph);

			display.Diagram = diagram;
		}

		private void InitShapes(Graph graph, Diagram diagram)
		{
			var node2shape = new Dictionary<Node, Shape>();
			var link2arrow = new Dictionary<Link, Shape>();

			foreach (var n in graph.Nodes) {
				var shape = (Ellipse)project.ShapeTypes["Ellipse"].CreateInstance();
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

		private void Layout(Diagram diagram, Graph graph)
		{
			var layouter = new FlowLayouter(project);
			layouter.AllShapes = diagram.Shapes;
			layouter.Shapes = diagram.Shapes;
			layouter.Prepare();
			layouter.Execute(10);
			layouter.Fit(ShapeSize, ShapeSize, diagram.Width - ShapeSize, diagram.Height - ShapeSize);
		}
	}
}
