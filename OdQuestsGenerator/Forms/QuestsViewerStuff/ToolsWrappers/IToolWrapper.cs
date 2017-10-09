using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataweb.NShape;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	interface IToolWrapper
	{
		event Action ToolDeselectedAutoCleared;

		void OnKeyUp(Keys keys);
		void OnShapesDeleted(List<Shape> affectedShapes);
		void OnShapesInserted(List<Shape> affectedShapes);
		void OnShapesUpdated(List<Shape> affectedShapes);
		void OnShapeClick(Shape shape);
		void OnShapeDoubleClick(Shape shape);
		void OnToolDeselected();
	}

	abstract class ToolWrapper<TTool> : IToolWrapper
		where TTool : Tool
	{
		protected readonly EditingContext Context;
		protected readonly TTool Tool;

		public event Action ToolDeselectedAutoCleared;

		public virtual void OnKeyUp(Keys keys) {}
		public virtual void OnShapesDeleted(List<Shape> affectedShapes) {}
		public virtual void OnShapesInserted(List<Shape> affectedShapes) {}
		public virtual void OnShapesUpdated(List<Shape> affectedShapes) {}
		public virtual void OnShapeClick(Shape shape) {}
		public virtual void OnShapeDoubleClick(Shape shape) {}

		public virtual void OnToolDeselected()
		{
			ToolDeselectedAutoCleared?.Invoke();
			ToolDeselectedAutoCleared = null;
		}

		protected ToolWrapper(EditingContext context, TTool tool)
		{
			Context = context;
			Tool = tool;
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
