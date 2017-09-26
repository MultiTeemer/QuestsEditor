using System.Collections.Generic;
using System.Windows.Forms;
using Dataweb.NShape;
using OdQuestsGenerator.Data;
using OdQuestsGenerator.Forms.QuestsViewerStuff.Commands;

namespace OdQuestsGenerator.Forms.QuestsViewerStuff.ToolsWrappers
{
	interface IToolWrapper
	{
		void KeyUp(Keys keys);
		void ShapesDeleted(List<Shape> affectedShapes);
		void ShapesInserted(List<Shape> affectedShapes);
		void ShapesUpdated(List<Shape> affectedShapes);
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

		public virtual void KeyUp(Keys keys) {}
		public virtual void ShapesDeleted(List<Shape> affectedShapes) {}
		public virtual void ShapesInserted(List<Shape> affectedShapes) {}
		public virtual void ShapesUpdated(List<Shape> affectedShapes) {}

		protected ToolWrapper(EditingContext context)
		{
			Context = context;
		}
	}
}
