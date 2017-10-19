using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataweb.NShape;
using OdQuestsGenerator.Commands;

namespace OdQuestsGenerator.Forms.BaseUIStuff.DiagramEditing
{
	interface IToolWrapper
	{
		event Action ToolDeselectedAutoCleared;

		void OnKeyUp(Keys keys);
		void OnShapesDeleted(List<Shape> affectedShapes);
		void OnShapesInserted(List<Shape> affectedShapes);
		void OnShapesUpdated(List<Shape> affectedShapes);
		void OnShapeMoved(Shape shape);
		void OnShapeClick(Shape shape);
		void OnShapeDoubleClick(Shape shape);
		void OnToolSelected();
		void OnToolDeselected();
	}

	abstract class ToolWrapper<TTool, TDiagramWrapper> : IToolWrapper
		where TTool : Tool
		where TDiagramWrapper : DiagramWrapper
	{
		protected readonly EditingContext Context;
		protected readonly TTool Tool;
		protected readonly TDiagramWrapper DiagramWrapper;

		public event Action ToolDeselectedAutoCleared;

		public virtual void OnKeyUp(Keys keys) {}
		public virtual void OnShapesDeleted(List<Shape> affectedShapes) {}
		public virtual void OnShapesInserted(List<Shape> affectedShapes) {}
		public virtual void OnShapesUpdated(List<Shape> affectedShapes) {}
		public virtual void OnShapeMoved(Shape shape) {}
		public virtual void OnShapeClick(Shape shape) {}
		public virtual void OnShapeDoubleClick(Shape shape) {}
		public virtual void OnToolSelected() {}

		public virtual void OnToolDeselected()
		{
			ToolDeselectedAutoCleared?.Invoke();
			ToolDeselectedAutoCleared = null;
		}

		protected ToolWrapper(EditingContext context, TTool tool, TDiagramWrapper diagramWrapper)
		{
			Context = context;
			Tool = tool;
			DiagramWrapper = diagramWrapper;
		}

		protected void DeleteShapeOnDeselect(Shape shape)
		{
			ToolDeselectedAutoCleared += () => shape.Diagram.Shapes.Remove(shape);
		}

		protected void DeleteShapesOnDeselect(IEnumerable<Shape> shapes)
		{
			foreach (var s in shapes) {
				ToolDeselectedAutoCleared += () => s.Diagram.Shapes.Remove(s);
			}
		}
	}
}
