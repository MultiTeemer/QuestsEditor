using System.Collections.Generic;
using Dataweb.NShape;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	interface IToolWrapper
	{
		void ShapesDeleted(List<Shape> affectedShapes);
		void ShapesInserted(List<Shape> affectedShapes);
		void ShapesUpdated(List<Shape> affectedShapes);
	}

	struct EditingContext
	{
		public readonly Project Project;
		public readonly CommandsHistory History;
		public readonly FlowView View;
		public readonly Code Code;
		public readonly CodeEditor CodeEditor;

		public EditingContext(Project project, CommandsHistory history, FlowView view, Code code, CodeEditor codeEditor)
		{
			Project = project;
			History = history;
			View = view;
			Code = code;
			CodeEditor = codeEditor;
		}
	}

	abstract class ToolWrapper : IToolWrapper
	{
		protected readonly EditingContext Context;

		public virtual void ShapesDeleted(List<Shape> affectedShapes) {}
		public virtual void ShapesInserted(List<Shape> affectedShapes) {}
		public virtual void ShapesUpdated(List<Shape> affectedShapes) {}

		protected ToolWrapper(EditingContext context)
		{
			Context = context;
		}
	}
}
