using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataweb.NShape;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	interface IToolWrapper
	{
		event Action ToolDeselectedAutoCleared;

		void OnKeyUp(Keys keys);
		void OnShapesDeleted(List<Shape> affectedShapes);
		void OnShapesInserted(List<Shape> affectedShapes);
		void OnShapesUpdated(List<Shape> affectedShapes);
		void OnToolDeselected();
	}

	class EditingContext
	{
		public readonly Project Project;
		public readonly CommandsHistory History;
		public readonly FlowView FlowView;
		public readonly Code Code;
		public readonly CodeEditor CodeEditor;
		public readonly ToolsManager ToolsManager;

		public Flow Flow;

		public EditingContext(
			Flow flow,
			Project project,
			CommandsHistory history,
			FlowView view,
			Code code,
			CodeEditor codeEditor,
			ToolsManager toolsManager
		)
		{
			Flow = flow;
			Project = project;
			History = history;
			FlowView = view;
			Code = code;
			CodeEditor = codeEditor;
			ToolsManager = toolsManager;
		}
	}

	abstract class ToolWrapper : IToolWrapper
	{
		protected readonly EditingContext Context;

		public event Action ToolDeselectedAutoCleared;

		public virtual void OnKeyUp(Keys keys) {}
		public virtual void OnShapesDeleted(List<Shape> affectedShapes) {}
		public virtual void OnShapesInserted(List<Shape> affectedShapes) {}
		public virtual void OnShapesUpdated(List<Shape> affectedShapes) {}

		public virtual void OnToolDeselected()
		{
			ToolDeselectedAutoCleared?.Invoke();
			ToolDeselectedAutoCleared = null;
		}

		protected ToolWrapper(EditingContext context)
		{
			Context = context;
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
